using System;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform is null || _context.Platforms is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            if (_context.Platforms is null) return new List<Platform>();
            return _context.Platforms.ToList();
        }

        public Platform? GetPlatformById(int id)
        {
            if (_context.Platforms == null) return null;
            return _context.Platforms.FirstOrDefault(x => x.Id == id);

        }

        public bool SaveChanges() => (_context.SaveChanges() >= 0);
    }
}
