using System.Data;

namespace CH.Alika.Json.Server.Model
{
    interface IRecordFactory
    {
        JRecord create(IDataRecord record);
    }
}
