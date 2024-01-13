using MongoDB.Bson;

namespace Model.BsonModel;

public class BaseBsonModel
{
    public ObjectId Id { get; protected set; }
}