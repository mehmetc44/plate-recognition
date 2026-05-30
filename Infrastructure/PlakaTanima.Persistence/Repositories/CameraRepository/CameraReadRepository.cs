using System;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.CameraRepository;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.CameraRepository;

public class CameraReadRepository : ReadRepository<Camera>, ICameraReadRepository
    {
        private readonly AppDbContext _context;

        public CameraReadRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Camera?> GetByIpAsync(string ip)
        {
            return _context.Set<Camera>()
                .FirstOrDefaultAsync(x => x.Ip == ip);
        }

        public Task<Camera?> GetByNameAsync(string name)
        {
            return _context.Set<Camera>()
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public Task<List<Camera>> GetActiveCamerasAsync()
        {
            return _context.Set<Camera>()
                .Where(x => x.Status == CameraStatus.Active)
                .ToListAsync();
        }

        public Task<List<Camera>> GetByGateNameAsync(string gateName)
        {
            return _context.Set<Camera>()
                .Where(x => x.GateName == gateName)
                .ToListAsync();
        }

        public Task<bool> ExistsByIpAsync(string ip)
        {
            return _context.Set<Camera>()
                .AnyAsync(x => x.Ip == ip);
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return _context.Set<Camera>()
                .AnyAsync(x => x.Name == name);
        }
    }
