namespace MunicipalityRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.NetTopology;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using NetTopologySuite.IO;
    using Newtonsoft.Json;

    public sealed class ExtendedWkbGeometry : ByteArrayValueObject<ExtendedWkbGeometry>
    {
        public const int SridLambert72 = SystemReferenceId.SridLambert72;
        private static readonly WKBWriter WkbWriter = new WKBWriter { Strict = false, HandleSRID = true };

        [JsonConstructor]
        public ExtendedWkbGeometry([JsonProperty("value")] byte[] ewkbBytes) : base(ewkbBytes) { }

        public ExtendedWkbGeometry(string ewkbBytesHex) : base(ewkbBytesHex.ToByteArray()!) { }

        public override string ToString() => Value.ToHexString()!;

        /// <summary>
        /// Create an ExtendedWkbGeometry from a WKB or an EWKB byte array.
        /// If the WKB does not contain an SRID, the method will attempt to read the geometry using the specified SRID (defaulting to Lambert72).
        /// If the SRID in the EWKB does not match the expected SRID, an InvalidOperationException is thrown.
        /// </summary>
        /// <param name="wkb"></param>
        /// <param name="useSrid"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ExtendedWkbGeometry? CreateEWkb(byte[]? wkb, int useSrid = SridLambert72)
        {
            if (wkb == null)
            {
                return null;
            }

            try
            {
                if (!wkb.TryReadSrid(out var srid))
                {
                    if (useSrid == SridLambert72)
                    {
                        var geometry = WKBReaderFactory.CreateForLambert72().Read(wkb);
                        return new ExtendedWkbGeometry(WkbWriter.Write(geometry));
                    }

                    if (useSrid == SystemReferenceId.SridLambert2008)
                    {
                        var geometry = WKBReaderFactory.CreateForLambert2008().Read(wkb);
                        return new ExtendedWkbGeometry(WkbWriter.Write(geometry));
                    }

                    return null;
                }

                if (srid != useSrid)
                    throw new InvalidOperationException("SRID in EWKB does not match the expected SRID.");

                var reader = WKBReaderFactory.CreateForEwkb(wkb);
                var ewkbGeometry = reader.Read(wkb);
                ewkbGeometry.SRID = srid;

                return new ExtendedWkbGeometry(WkbWriter.Write(ewkbGeometry));
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
