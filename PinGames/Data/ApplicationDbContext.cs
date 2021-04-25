using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PinGames.Models;

namespace PinGames.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public virtual DbSet<UserAccountModel> Users { get; set; }
        public virtual DbSet<GameModel> Games { get; set; }
        public virtual DbSet<ReviewModel> Reviews { get; set; }
        public virtual DbSet<GenreModel> Genres { get; set; }
        public virtual DbSet<LibraryModel> Libraries { get; set; }
    }
}
