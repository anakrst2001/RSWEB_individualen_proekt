using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecordStore.Areas.Identity.Data;
using RecordStore.Models;

namespace RecordStore.Data
{
    public class RecordStoreContext : IdentityDbContext<RecordStoreUser>
    {
        public RecordStoreContext (DbContextOptions<RecordStoreContext> options)
            : base(options)
        {
        }

        public DbSet<RecordStore.Models.Song> Song { get; set; } = default!;

        public DbSet<RecordStore.Models.Artist>? Artist { get; set; }

        public DbSet<RecordStore.Models.Album>? Album { get; set; }

        public DbSet<RecordStore.Models.Review>? Review { get; set; }

        public DbSet<RecordStore.Models.UserAlbum>? UserAlbum { get; set; }

        public DbSet<RecordStore.Models.Genre>? Genre { get; set; }

        public DbSet<RecordStore.Models.SongGenre> SongGenre { get; set; }

        public DbSet<RecordStore.Models.AlbumGenre> AlbumGenre { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SongGenre>()
            .HasOne<Song>(p => p.Song)
            .WithMany(p => p.Genres)
            .HasForeignKey(p => p.SongId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<SongGenre>()
            .HasOne<Genre>(p => p.Genre)
            .WithMany(p => p.Songs)
            .HasForeignKey(p => p.GenreId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<AlbumGenre>()
            .HasOne<Album>(p => p.Album)
            .WithMany(p => p.Genres)
            .HasForeignKey(p => p.AlbumId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<AlbumGenre>()
            .HasOne<Genre>(p => p.Genre)
            .WithMany(p => p.Albums)
            .HasForeignKey(p => p.GenreId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Song>()
            .HasOne<Artist>(p => p.Artist)
            .WithMany(p => p.Songs)
            .HasForeignKey(p => p.ArtistId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Song>()
            .HasOne<Album>(p => p.Album)
            .WithMany(p => p.Songs)
            .HasForeignKey(p => p.AlbumId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Album>()
            .HasOne<Artist>(p => p.Artist)
            .WithMany(p => p.Albums)
            .HasForeignKey(p => p.ArtistId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Review>()
            .HasOne<Album>(p => p.Album)
            .WithMany(p => p.Reviews)
            .HasForeignKey(p => p.AlbumId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<UserAlbum>()
            .HasOne<Album>(p => p.Album)
            .WithMany(p => p.UserAlbums)
            .HasForeignKey(p => p.AlbumId);
            //.HasPrincipalKey(p => p.Id);
        }

    }
}
