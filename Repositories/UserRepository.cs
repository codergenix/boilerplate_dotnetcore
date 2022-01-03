using DotNetCoreBoilerPlate.Interfaces;
using DotNetCoreBoilerPlate.Models;
using DotNetCoreBoilerPlate.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreBoilerPlate.Repositories
{
    public class UserRepository : IUser
    {
        NetCoreBoilerPlateContext _db;
        public UserRepository(NetCoreBoilerPlateContext db)
        {
            _db = db;
        }

        public async Task<Tuple<List<UserViewModel>, int>> GetAll(string order, string orderbyval, string searchValue, int skip, int pageSize)
        {
            if (_db != null)
            {
                List<UserViewModel> recordList;
                int count = 0;

                bool isDescending = false;
                if (order.ToLower().Equals("desc"))
                    isDescending = true;
                if (string.IsNullOrEmpty(searchValue))
                {
                    recordList = await (from pu in _db.BoilerTables

                                        select new UserViewModel
                                        {
                                            ID = pu.ID,
                                            FirstName = pu.FirstName,
                                            LastName = pu.LastName,
                                            Email = pu.Email,
                                            Description = pu.Description
                                        }).AsQueryable().OrderBy(orderbyval, isDescending).ToListAsync();
                }
                else
                {
                    recordList = await (from pu in _db.BoilerTables

                                        where (EF.Functions.Like(pu.FirstName, "%" + searchValue + "%") ||
                                        EF.Functions.Like(pu.LastName, "%" + searchValue + "%") ||
                                        EF.Functions.Like(pu.Email, "%" + searchValue + "%"))
                                        select new UserViewModel
                                        {
                                            ID = pu.ID,
                                            FirstName = pu.FirstName,
                                            LastName = pu.LastName,
                                            Email = pu.Email,
                                            Description = pu.Description
                                        }).AsQueryable().OrderBy(orderbyval, isDescending).ToListAsync();
                }
                count = recordList.Count;
                recordList = recordList.Skip(skip).Take(pageSize).ToList();

                return Tuple.Create(recordList, count);
            }
            return null;
        }

        public async Task<UserViewModel> AddUser(UserViewModel user)
        {
            if (_db != null)
            {
                BoilerTable _user = new BoilerTable();
                _user.FirstName = user.FirstName;
                _user.LastName = user.LastName;
                _user.Email = user.Email;
                _user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _user.Description = user.Description;

                _db.BoilerTables.Add(_user);
                await _db.SaveChangesAsync();

                var recordList = await (from pu in _db.BoilerTables

                                        where pu.ID == _user.ID
                                        select new UserViewModel
                                        {
                                            ID = pu.ID,
                                            FirstName = pu.FirstName,
                                            LastName = pu.LastName,
                                            Email = pu.Email,
                                            Description = pu.Description
                                        }).FirstOrDefaultAsync();
                return recordList;
            }
            return null;
        }

        public async Task<UserViewModel> UpdateUser(UserViewModel user)
        {
            if (_db != null)
            {
                var pphs = await _db.BoilerTables.FirstOrDefaultAsync(x => x.ID == user.ID);

                if (pphs != null)
                {
                    pphs.FirstName = user.FirstName;
                    pphs.LastName = user.LastName;
                    pphs.Email = user.Email;
                    pphs.Description = user.Description;
                    if (!string.IsNullOrEmpty(user.Password))
                        pphs.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    pphs.updateBy = user.ID.ToString();

                    _db.BoilerTables.Update(pphs);
                    await _db.SaveChangesAsync();

                    var recordList = await (from pu in _db.BoilerTables

                                            where pu.ID == pphs.ID
                                            select new UserViewModel
                                            {
                                                ID = pu.ID,
                                                FirstName = pu.FirstName,
                                                LastName = pu.LastName,
                                                Email = pu.Email,
                                                Description = pu.Description
                                            }).FirstOrDefaultAsync();
                    return recordList;
                }
                return null;
            }
            return null;
        }

        public async Task<int> DeleteUser(int userID)
        {
            int result = 0;

            if (_db != null)
            {
                var pphs = await _db.BoilerTables.FirstOrDefaultAsync(x => x.ID == userID);

                if (pphs != null)
                {
                    pphs.deletedAt = DateTime.Now;

                    _db.BoilerTables.Update(pphs);

                    result = await _db.SaveChangesAsync();
                }
                return result;
            }

            return result;
        }
    }
}
