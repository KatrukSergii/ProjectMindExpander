using Shared.Enum;

namespace Shared.DataStructures
{
    public class PropertyData
    {
        public PropertyData()
        {
           
        }

        public PropertyData(string typeName, PropertyType propertyType, string genericTypeName,
                            string genericTypeParameterName)
        {
            TypeName = typeName;
            PropertyType = propertyType;
            GenericTypeName = genericTypeName;
            GenericTypeParameterName = genericTypeParameterName;
        }

        public string TypeName { get; set; }
        public PropertyType PropertyType { get; set; }
        public string GenericTypeName { get; set; }
        public string GenericTypeParameterName { get; set; }
    }
}
