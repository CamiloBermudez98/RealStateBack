namespace RealEstateAPI.Services
{
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using RealEstateAPI.Data;
    using RealEstateAPI.Models;

    public class PropertyService
    {
        private readonly IMongoCollection<Property> _propertyCollection;

        public PropertyService(IOptions<RealEstateDatabaseSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _propertyCollection = database.GetCollection<Property>(settings.Value.PropertyCollection);
        }
        public async Task<List<Property>> GetAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            var filter = Builders<Property>.Filter.Empty;

            if (!string.IsNullOrEmpty(name))
                filter &= Builders<Property>.Filter.Regex("Name", new BsonRegularExpression(name, "i"));

            if (!string.IsNullOrEmpty(address))
                filter &= Builders<Property>.Filter.Regex("Location.Address", new BsonRegularExpression(address, "i"));

            if (minPrice.HasValue)
                filter &= Builders<Property>.Filter.Gte(p => p.Price, minPrice.Value);

            if (maxPrice.HasValue)
                filter &= Builders<Property>.Filter.Lte(p => p.Price, maxPrice.Value);

            return await _propertyCollection.Find(filter).ToListAsync();
        }
        public async Task<List<Property>> GetAsync() =>
            await _propertyCollection.Find(_ => true).ToListAsync();

        public async Task<Property> GetByIdAsync(string id) =>
            await _propertyCollection.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Property newProperty) =>
            await _propertyCollection.InsertOneAsync(newProperty);

        public async Task UpdateAsync(string id, Property updatedProperty) =>
            await _propertyCollection.ReplaceOneAsync(p => p.Id == id, updatedProperty);

        public async Task DeleteAsync(string id) =>
            await _propertyCollection.DeleteOneAsync(p => p.Id == id);
    }
}
