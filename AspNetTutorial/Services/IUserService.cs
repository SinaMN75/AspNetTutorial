namespace AspNetTutorial.Services;

using Dtos;
using Entities;
using Microsoft.EntityFrameworkCore;

public interface IUserService {
	Task<IEnumerable<UserResponse>> Read();
	Task<UserResponse?> ReadById(Guid i);
	Task<UserResponse?> Update(UserUpdateParams param);
	Task Delete(Guid id);
	Task<UserResponse> Create(UserCreateParams user);

	Task<UserResponse?>? GetProfile();
}

public class UserService(AppDbContext dbContext, IHttpContextAccessor httpContext) : IUserService {
	public async Task<IEnumerable<UserResponse>> Read() {
		List<UserResponse> list = await dbContext.Users.Select(x => new UserResponse {
			Id = x.Id,
			FullName = x.FullName,
			Email = x.Email,
			PhoneNumber = x.PhoneNumber,
			Birthdate = x.Birthdate,
			IsMarried = x.IsMarried,
			Age = 7,
			Classes = x.Classes.ToList()
		}).ToListAsync();

		return list;
	}

	public async Task<UserResponse?> ReadById(Guid id) {
		UserEntity? user = await dbContext.Users.FindAsync(id);
		if (user == null) {
			return null;
		}

		int? age = null;
		if (user.Birthdate != null) {
			age = DateTime.UtcNow.Year - user.Birthdate.Value.Year;
		}

		UserResponse response = new() {
			Id = user.Id,
			FullName = user.FullName,
			Email = user.Email,
			PhoneNumber = user.PhoneNumber,
			Birthdate = user.Birthdate,
			IsMarried = user.IsMarried,
			Age = 7
		};

		return response;
	}

	public async Task<UserResponse?> Update(UserUpdateParams param) {
		UserEntity? user = await dbContext.Users.FindAsync(param.Id);
		if (user == null) return null;
		if (param.IsMarried != null) user.IsMarried = param.IsMarried.Value;
		if (param.PhoneNumber != null) user.PhoneNumber = param.PhoneNumber;
		if (param.Birthdate != null) user.Birthdate = param.Birthdate;
		if (param.FullName != null) user.FullName = param.FullName;
		if (param.Email != null) user.Email = param.Email;

		dbContext.Users.Update(user);
		await dbContext.SaveChangesAsync();
		return new UserResponse {
			Id = user.Id,
			FullName = user.FullName,
			Email = user.Email,
			PhoneNumber = user.PhoneNumber,
			Birthdate = user.Birthdate,
			IsMarried = user.IsMarried,
			Age = 7
		};
	}

	public async Task Delete(Guid id) {
		UserEntity? user = await dbContext.Users.FindAsync(id);
		if (user == null) return;
		dbContext.Users.Remove(user);
		await dbContext.SaveChangesAsync();
	}

	public async Task<UserResponse> Create(UserCreateParams dto) {
		UserEntity user = new() {
			Id = Guid.CreateVersion7(),
			FullName = dto.FullName,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			Birthdate = dto.Birthdate,
			IsMarried = dto.IsMarried,
			Password = "123456789"
		};
		UserEntity entity = dbContext.Users.Add(user).Entity;
		await dbContext.SaveChangesAsync();

		int? age = null;
		if (entity.Birthdate != null) {
			age = DateTime.UtcNow.Year - entity.Birthdate.Value.Year;
		}

		return new UserResponse {
			Id = entity.Id,
			FullName = entity.FullName,
			Email = entity.Email,
			PhoneNumber = entity.PhoneNumber,
			Birthdate = entity.Birthdate,
			IsMarried = entity.IsMarried,
			Age = age
		};
	}

	public async Task<UserResponse?>? GetProfile() {
		string? userId = httpContext.HttpContext.User.Identity.Name;
		if (userId == null) return null;
		UserResponse? user = await ReadById(Guid.Parse(userId));
		return user ?? null;
	}
}