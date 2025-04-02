namespace AspNetTutorial.Services;

using Dtos;
using Entities;
using Microsoft.EntityFrameworkCore;

public interface IUserService {
	Task<IEnumerable<UserResponse>> Read(CancellationToken ct);
	Task<IEnumerable<UserEntity>> Filter(UserFilterParams param, CancellationToken ct);
	Task<BaseResponse<UserResponse?>> ReadById(Guid i, CancellationToken ct);
	Task<UserResponse?> Update(UserUpdateParams param, CancellationToken ct);
	Task Delete(Guid id, CancellationToken ct);
	Task<UserResponse> Create(UserCreateParams user, CancellationToken ct);
	Task<UserResponse?>? GetProfile(CancellationToken ct);
}

public class UserService(AppDbContext dbContext, IHttpContextAccessor httpContext) : IUserService {
	public async Task<IEnumerable<UserResponse>> Read(CancellationToken ct) {
		// IEnumerable<UserEntity> list =
		// 	await dbContext.Users.Select(x => new UserEntity {
		// 		FullName = x.FullName,
		// 		Email = x.Email,
		// 		PhoneNumber = x.PhoneNumber,
		// 		Password = "",
		// 		Classes = x.Classes.Select(c => new ClassEntity {
		// 			Id = c.Id,
		// 			Title = c.Title,
		// 			Subject = c.Subject
		// 		})
		// 	}).ToListAsync();

		// IEnumerable<UserEntity> list = await dbContext.Users.Include(x => x.Classes).ToListAsync();


		List<UserResponse> list = await dbContext.Users.Select(x => new UserResponse {
			Id = x.Id,
			FullName = x.FullName,
			Email = x.Email,
			PhoneNumber = x.PhoneNumber,
			Birthdate = x.Birthdate,
			IsMarried = x.IsMarried,
			Age = 7,
			Classes = x.Classes.ToList()
		}).OrderBy(x => x.PhoneNumber).ToListAsync(ct);

		return new List<UserResponse>();
	}

	public async Task<IEnumerable<UserEntity>> Filter(UserFilterParams p, CancellationToken ct) {
		IQueryable<UserEntity> q = dbContext.Users;
		if (!string.IsNullOrWhiteSpace(p.Email)) q = q.Where(x => x.Email.Contains(p.Email));
		if (!string.IsNullOrWhiteSpace(p.FatherName)) q = q.Where(x => x.JsonDetail.FatherName.Contains(p.FatherName));
		if (p.Point != null) q = q.Where(x => x.JsonDetail.Point == p.Point);
		if (p.Tags != null) q = q.Where(x => x.Tags.Any(tag => p.Tags.Contains(tag)));
		
		return await q.ToListAsync();
	}

	public async Task<BaseResponse<UserResponse?>> ReadById(Guid id, CancellationToken ct) {
		UserEntity? user = await dbContext.Users.FindAsync(id, ct);
		if (user == null) {
			return new BaseResponse<UserResponse?>(null, 404, "User not found");
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

		return new BaseResponse<UserResponse?>(response);
	}

	public async Task<UserResponse?> Update(UserUpdateParams param, CancellationToken ct) {
		// int count = await dbContext.Users.Where(x => x.Id == param.Id).ExecuteUpdateAsync(x =>
		// x.SetProperty(x => x.Email, param.Email)
		// .SetProperty(x => x.Birthdate, param.Birthdate));

		UserEntity? user = await dbContext.Users.Include(x => x.Classes).FirstOrDefaultAsync(x => x.Id == param.Id, ct);
		if (user == null) return null;
		if (param.IsMarried != null) user.IsMarried = param.IsMarried.Value;
		if (param.PhoneNumber != null) user.PhoneNumber = param.PhoneNumber;
		if (param.Birthdate != null) user.Birthdate = param.Birthdate;
		if (param.FullName != null) user.FullName = param.FullName;
		if (param.Email != null) user.Email = param.Email;

		dbContext.Users.Update(user);
		await dbContext.SaveChangesAsync(ct);
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

	public async Task Delete(Guid id, CancellationToken ct) {
		// int count = await dbContext.Users.Where(x => x.Id == id).ExecuteDeleteAsync();

		UserEntity? user = await dbContext.Users.FindAsync(id);
		if (user == null) return;
		dbContext.Users.Remove(user);
		await dbContext.SaveChangesAsync(ct);
	}

	public async Task<UserResponse> Create(UserCreateParams dto, CancellationToken ct) {
		UserEntity user = new() {
			Id = Guid.CreateVersion7(),
			FullName = dto.FullName,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			Birthdate = dto.Birthdate,
			IsMarried = dto.IsMarried,
			Password = "123456789",
			Tags = dto.Tags,
			JsonDetail = new UserJsonDetail {
				FatherName = dto.FatherName ?? "",
				Point = dto.Point ?? 0
			}
		};
		UserEntity entity = dbContext.Users.Add(user).Entity;
		await dbContext.SaveChangesAsync(ct);

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

	public async Task<UserResponse?>? GetProfile(CancellationToken ct) {
		string? userId = httpContext.HttpContext.User.Identity.Name;
		if (userId == null) return null;
		BaseResponse<UserResponse?> user = await ReadById(Guid.Parse(userId), ct);
		return user.Result ?? null;
	}
}