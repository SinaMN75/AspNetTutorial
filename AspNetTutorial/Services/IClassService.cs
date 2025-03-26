namespace AspNetTutorial.Services;

using Dtos;
using Entities;
using Microsoft.EntityFrameworkCore;

public interface IClassService {
	Task<List<ClassEntity>> Read();
	Task<ClassEntity?> ReadById(Guid i);
	Task<ClassEntity?> Update(ClassEntity param);
	Task Delete(Guid id);
	Task<ClassEntity> Create(ClassCreateDto param);
}

public class ClassService(AppDbContext dbContext) : IClassService {
	public async Task<List<ClassEntity>> Read() {
		List<ClassEntity> list = await dbContext.Class
			.Include(x => x.School)
			.Include(x=> x.Users)
			.ToListAsync();
		return list;
	}

	public async Task<ClassEntity?> ReadById(Guid id) {
		ClassEntity? e = await dbContext.Class.FindAsync(id);
		return e ?? null;
	}

	public async Task<ClassEntity?> Update(ClassEntity param) {
		ClassEntity? e = await dbContext.Class.FindAsync(param.Id);
		if (e == null) return null;
		e.Title = param.Title;
		e.Subject = param.Subject;
		if (param.SchoolId != null) e.SchoolId = param.SchoolId;

		dbContext.Class.Update(e);
		await dbContext.SaveChangesAsync();
		return new ClassEntity {
			Id = e.Id,
			Title = e.Title,
			Subject = e.Subject,
			SchoolId = e.SchoolId
		};
	}

	public async Task Delete(Guid id) {
		ClassEntity? e = await dbContext.Class.FindAsync(id);
		if (e == null) return;
		dbContext.Class.Remove(e);
		await dbContext.SaveChangesAsync();
	}

	public async Task<ClassEntity> Create(ClassCreateDto param) {
		List<UserEntity> userList = [];
		foreach (Guid userId in param.Users) {
			UserEntity? user = await dbContext.Users.FindAsync(userId);
			userList.Add(user);
		}
		
		
		ClassEntity e = new() {
			Id = Guid.CreateVersion7(),
			Subject = param.Subject,
			Title = param.Title,
			SchoolId = param.SchoolId,
			Users = userList
		};
		ClassEntity entity = dbContext.Class.Add(e).Entity;
		await dbContext.SaveChangesAsync();

		return entity;
	}
}