﻿namespace FlashLFQ
{
    public class SpectraFileInfo
    {
        public readonly string fullFilePathWithExtension;
        public readonly string filenameWithoutExtension;

        // condition/biorep/techrep/fraction info
        public readonly string condition;

        public readonly int biologicalReplicate;
        public readonly int fraction;
        public readonly int technicalReplicate;

        public SpectraFileInfo(string fullFilePathWithExtension, string condition, int biorep, int techrep, int fraction)
        {
            this.fullFilePathWithExtension = fullFilePathWithExtension;
            this.filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(this.fullFilePathWithExtension);
            this.condition = condition;
            this.biologicalReplicate = biorep;
            this.technicalReplicate = techrep;
            this.fraction = fraction;
        }

        // files are considered the same if the absolute file path is the same
        public override bool Equals(object obj)
        {
            return base.Equals(obj) && ((SpectraFileInfo)obj).fullFilePathWithExtension.Equals(this.fullFilePathWithExtension);
        }

        public override int GetHashCode()
        {
            return fullFilePathWithExtension.GetHashCode();
        }
    }
}