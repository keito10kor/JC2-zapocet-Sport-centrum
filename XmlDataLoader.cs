using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SportCentrum.DtoModels;

public static class XmlDataLoader
{
    public static DataDto LoadFromFile(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var serializer = new XmlSerializer(typeof(DataDto));
        return (DataDto)serializer.Deserialize(reader);
    }
}
