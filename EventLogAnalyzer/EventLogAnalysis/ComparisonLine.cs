﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fastenshtein;

namespace EventLogAnalysis
{
    public class ComparisonLine
    {
        // is there any use in considering string interning as part of optimization?
        // helper: https://github.com/astef/InternedString
        // for events that do actually use the exact same message, this would prevent multiple instances and make for quick comparisons
        // but if you don't know ahead of time, then maybe this has no role to play

        public ComparisonLine(ELRecord record)
        {
            ComparisonRecord = record;

            if (string.IsNullOrEmpty(ComparisonRecord.GetMessage()))
            {
                Debug.WriteLine($"null lstring message");
            }

            Lev = new(ComparisonRecord.GetMessage());

            TraitValue = $"{ComparisonRecord.ProviderName}-{ComparisonRecord.Record.Id}-{ComparisonRecord.Message}";
        }

        public static bool CheckForExactMatch { get; set; } = false;
        public static double Threshold { get; set; } = 0.6;
        public ELRecord ComparisonRecord { get; init; }

        public Levenshtein Lev { get; init; }

        public string TraitValue { get; }

        public bool SimilarEnough(ELRecord record, out double similarityPercentage)
        {
            var bothBlank = string.IsNullOrWhiteSpace(record.Message) && string.IsNullOrWhiteSpace(record.Message);
            if (bothBlank || (CheckForExactMatch && ComparisonRecord.Message == record.Message))
            {
                record.GroupSimilarity = 1;
                similarityPercentage = 1.0;
                return true;
            }

            similarityPercentage = LevPercent(record.Message);
            if (similarityPercentage > Threshold)
            {
                record.GroupSimilarity = similarityPercentage;
                return true;
            }

            return false;
        }

        public bool SimilarEnough(ComparisonLine other, out double similarityPercentage)
        {
            return SimilarEnough(other.ComparisonRecord, out similarityPercentage);
        }

        private double LevPercent(string compare)
        {
            return Math.Abs(1 - (double)Lev.DistanceFrom(compare) / (double)Math.Max(Lev.StoredLength, compare.Length));
        }
    }
}