namespace MunicipalityRegistry.Municipality
{
    using NetTopologySuite.Geometries;

    public static class GeometryValidator
    {
        public static bool IsValid(Geometry geometry)
        {
            var validOp =
                new NetTopologySuite.Operation.Valid.IsValidOp(geometry)
                {
                    SelfTouchingRingFormingHoleValid = true
                };

            return validOp.IsValid;
        }
    }
}
