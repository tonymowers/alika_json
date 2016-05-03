using System.Data;

namespace CH.Alika.Json.Server.Model
{
    interface IRecordFactory
    {
        IRecord Create(IDataRecord record);
    }
}
