using Infrastructure.Persistence.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Model;
using Shared.Exceptions;
using Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;

public class StaffManager
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    public StaffManager(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Staff> CreateAdminStaffAsync(
        string name,
        string email,
        string password,
        string? description)
    {
        if (string.IsNullOrEmpty(name))
            throw new BadRequestException("Name required");

        if (string.IsNullOrEmpty(email))
            throw new BadRequestException("Email required");

        if (string.IsNullOrEmpty(password))
            throw new BadRequestException("Password required");

        var adminAuthRole = AuthRole.Admin;

        var staff = new Staff(
            name,
            email,
            "not hash",
            description,
            adminAuthRole);

        staff.UpdatePassword(HashPassword(staff, password));

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.Staffs.Add(staff);

        await dbContext.SaveChangesAsync();

        return staff;
    }

    public async Task<Staff> CreateUserStaffAsync(
        string name,
        string email,
        string password,
        string? description)
    {
        if (string.IsNullOrEmpty(name))
            throw new BadRequestException("Name required");

        if (string.IsNullOrEmpty(email))
            throw new BadRequestException("Email required");

        if (string.IsNullOrEmpty(password))
            throw new BadRequestException("Password required");

        var userAuthRole = AuthRole.User;

        var staff = new Staff(
            name,
            email,
            "not hash",
            description,
            userAuthRole);

        staff.UpdatePassword(HashPassword(staff, password));

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.Staffs.Add(staff);

        await dbContext.SaveChangesAsync();

        return staff;
    }

    public async Task<Staff> FindStaffAsync(long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var staff = await dbContext.Staffs.AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException("Staff not found!");

        if (VerifyAuthRole(staff.AuthRole) == false)
            throw new NotFoundException("Role not found!");

        return staff;
    }

    public async Task<Staff> FindStaffAsync(string name)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var staff = await dbContext.Staffs.AsNoTracking()
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException("Staff not found!");

        if (VerifyAuthRole(staff.AuthRole) == false)
            throw new NotFoundException("Role not found!");

        return staff;
    }

    public async Task<bool> IsExistStaffNameAsync(string name)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Staffs.AnyAsync(x => x.Name == name);
    }

    public async Task<bool> VerifyPasswordAsync(Staff staff, string password)
    {
        var staffPasswordHasher = new PasswordHasher<Staff>();

        var verifyHashedPasswordResult = staffPasswordHasher.VerifyHashedPassword(
            staff, staff.Password, password);

        if (verifyHashedPasswordResult == PasswordVerificationResult.Failed)
            return false;

        if (verifyHashedPasswordResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var staffContext = await dbContext.Staffs
                .Where(x => x.Id == staff.Id)
                .FirstOrDefaultAsync();

            if (staffContext == null)
                return false;

            staffContext.UpdatePassword(staffPasswordHasher.HashPassword(staffContext, password));

            await dbContext.SaveChangesAsync();
        }

        return true;
    }

    public async Task<(List<Staff> Data, int TotalCount)> SearchUserStaffs(int page, int size)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var staffQuery = dbContext.Staffs.AsNoTracking()
            .Where(x => x.AuthRole == AuthRole.User);

        var staffs = await staffQuery
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalCount = await staffQuery.CountAsync();

        return (staffs, totalCount);
    }

    private string HashPassword(Staff staff, string password)
    {
        var staffPasswordHasher = new PasswordHasher<Staff>();

        return staffPasswordHasher.HashPassword(staff, password);
    }

    private bool VerifyAuthRole(string staffAuthRole)
    {
        return AuthRole.GetAuthRoles().Any(x => x == staffAuthRole);
    }
}
