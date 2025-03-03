internal static class DiagnosticIds
{
    internal static class Input
    {
        public static readonly string PackageNotSpecified = "VNS0001";
        public static readonly string PackageNotFound = "VNS0002";
        public static readonly string BaselineDirectoryNotSpecified = "VNS0003";
    }

    internal static class Baseline
    {
        public static readonly string Mismatch = "VNS1001";
        public static readonly string Unknown = "VNS1002";
    }
}
