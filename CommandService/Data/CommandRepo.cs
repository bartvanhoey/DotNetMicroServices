using System.Linq;
using CommandService.Models;

namespace CommandService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context) => _context = context;

    public bool SaveChanges() => _context.SaveChanges() >= 0;

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms!.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null) throw new ArgumentNullException(nameof(platform));
        _context.Platforms?.Add(platform);
    }

    public bool PlatformExists(int platformId) 
        => (_context.Platforms ?? throw new InvalidOperationException()).Any(p => p.Id == platformId);

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return (_context.Commands ?? throw new InvalidOperationException())
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform!.Name);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands.Where(c => c.PlatformId == platformId &&  c.Id == commandId).FirstOrDefault() ?? throw new InvalidOperationException();
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }
}