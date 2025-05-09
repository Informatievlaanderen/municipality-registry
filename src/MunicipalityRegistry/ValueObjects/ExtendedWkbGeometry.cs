﻿namespace MunicipalityRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using NetTopologySuite.IO;
    using Newtonsoft.Json;

    public sealed class ExtendedWkbGeometry : ByteArrayValueObject<ExtendedWkbGeometry>
    {
        public const int SridLambert72 = 31370;

        private static readonly WKBReader WkbReader = GeometryConfiguration.CreateWkbReader();

        [JsonConstructor]
        public ExtendedWkbGeometry([JsonProperty("value")] byte[] ewkbBytes) : base(ewkbBytes) { }

        public ExtendedWkbGeometry(string ewkbBytesHex) : base(ewkbBytesHex.ToByteArray()!) { }

        public override string ToString() => Value.ToHexString()!;

        public static ExtendedWkbGeometry? CreateEWkb(byte[]? wkb)
        {
            if (wkb == null)
            {
                return null;
            }

            try
            {
                var geometry = WkbReader.Read(wkb);
                geometry.SRID = SpatialReferenceSystemId.Lambert72;

                var wkbWriter = new WKBWriter { Strict = false, HandleSRID = true };
                return new ExtendedWkbGeometry(wkbWriter.Write(geometry));
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
