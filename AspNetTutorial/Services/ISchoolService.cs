namespace AspNetTutorial.Services;

using Entities;
using Microsoft.EntityFrameworkCore;

public interface ISchoolService {
	Task<List<SchoolEntity>> Read();
	Task<SchoolEntity?> ReadById(Guid i);
	Task<SchoolEntity?> Update(SchoolEntity param);
	Task Delete(Guid id);
	Task<SchoolEntity> Create(SchoolEntity param);
}

public class SchoolService(AppDbContext dbContext) : ISchoolService {
	public async Task<List<SchoolEntity>> Read() {
		List<SchoolEntity> list = await dbContext.School.Include(x => x.Classes).ToListAsync();
		return list;
	}

	public async Task<SchoolEntity?> ReadById(Guid id) {
		SchoolEntity? e = await dbContext.School.FindAsync(id);
		return e ?? null;
	}

	public async Task<SchoolEntity?> Update(SchoolEntity param) {
		SchoolEntity? e = await dbContext.School.FindAsync(param.Id);
		if (e == null) return null;
		e.Title = param.Title;

		dbContext.School.Update(e);
		await dbContext.SaveChangesAsync();
		return new SchoolEntity {
			Id = e.Id,
			Title = e.Title,
		};
	}

	public async Task Delete(Guid id) {
		SchoolEntity? e = await dbContext.School.FindAsync(id);
		if (e == null) return;
		dbContext.School.Remove(e);
		await dbContext.SaveChangesAsync();
	}

	public async Task<SchoolEntity> Create(SchoolEntity param) {
		SchoolEntity e = new() {
			Id = Guid.CreateVersion7(),
			Title = param.Title,
		};
		SchoolEntity entity = dbContext.School.Add(e).Entity;
		await dbContext.SaveChangesAsync();

		return entity;
	}
}