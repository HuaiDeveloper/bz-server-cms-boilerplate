using Microsoft.Extensions.Options;
using Model.BsonModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.MongoDBDriver;

public class MongoDBContext
{
    private readonly MongoDBSetting _mongoDBSetting;
    private readonly IMongoDatabase _mongoDatabase;
    public MongoDBContext(IOptions<MongoDBSetting> mongoDBSettingOptions)
    {
        _mongoDBSetting = mongoDBSettingOptions.Value;
        var mongoClient = new MongoClient(
            _mongoDBSetting.ConnectionString);

        _mongoDatabase= mongoClient.GetDatabase(_mongoDBSetting.Database);
    }

    public IMongoCollection<Feedback> Feedbacks => _mongoDatabase.GetCollection<Feedback>(nameof(Feedback));
}