using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordStore.Areas.Identity.Data;
using RecordStore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecordStore.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<RecordStoreUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            RecordStoreUser user = await UserManager.FindByEmailAsync("admin@mvcrecordstore.com");
            if (user == null)
            {
                var User = new RecordStoreUser();
                User.Email = "admin@mvcrecordstore.com";
                User.UserName = "admin@mvcrecordstore.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            var roleCheck2 = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck2)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
            }
            RecordStoreUser user1 = await UserManager.FindByEmailAsync("user1@mvcrecordstore.com");
            if (user1 == null)
            {
                var User = new RecordStoreUser();
                User.Email = "user1@mvcrecordstore.com";
                User.UserName = "user1@mvcrecordstore.com";
                string userPWD = "User123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                if (chkUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(User, "User");
                }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RecordStoreContext(serviceProvider.GetRequiredService<DbContextOptions<RecordStoreContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                if (context.Song.Any() || context.Artist.Any() || context.Album.Any() || context.Review.Any() || context.UserAlbum.Any() || context.Genre.Any())
                {
                    return; // DB has been seeded
                }

                context.Artist.AddRange(
                    new Artist { /*Id = 1, */FirstName = "Gordon", LastName = "Sumner", BirthDate = DateTime.Parse("1951-10-2"), Nationality="English", Gender="male"},
                    new Artist { /*Id = 2, */FirstName = "Bryan", LastName = "Adams", BirthDate = DateTime.Parse("1959-11-5"), Nationality = "Canadian", Gender = "male" },
                    new Artist { /*Id = 3, */FirstName = "George", LastName = "Michael", BirthDate = DateTime.Parse("1963-6-25"), Nationality = "English", Gender = "male" },
                    new Artist { /*Id = 4, */FirstName = "Freddie", LastName = "Mercury", BirthDate = DateTime.Parse("1946-9-5"), Nationality = "British", Gender = "male" },
                    new Artist { /*Id = 5, */FirstName = "Robert", LastName = "Smith", BirthDate = DateTime.Parse("1959-4-21"), Nationality = "English", Gender = "male" },
                    new Artist { /*Id = 6, */FirstName = "Jon", LastName = "Bon Jovi", BirthDate = DateTime.Parse("1962-3-2"), Nationality = "American", Gender = "male" },
                    new Artist { /*Id = 7, */FirstName = "Michael", LastName = "Hutchence", BirthDate = DateTime.Parse("1960-1-22"), Nationality = "Australian", Gender = "male" },
                    new Artist { /*Id = 8, */FirstName = "Dave", LastName = "Gahan", BirthDate = DateTime.Parse("1962-5-9"), Nationality = "English", Gender = "male" },
                    new Artist { /*Id = 9, */FirstName = "Steven", LastName = "Morrissey", BirthDate = DateTime.Parse("1959-5-22"), Nationality = "English", Gender = "male" },
                    new Artist { /*Id = 10, */FirstName = "Alex", LastName = "Turner", BirthDate = DateTime.Parse("1986-1-6"), Nationality = "English", Gender = "male" },
                    new Artist { /*Id = 11, */FirstName = "Ryan", LastName = "Tedder", BirthDate = DateTime.Parse("1979-6-26"), Nationality = "American", Gender = "male" },
                    new Artist { /*Id = 12, */FirstName = "Annie", LastName = "Lennox", BirthDate = DateTime.Parse("1954-12-25"), Nationality = "Scottish", Gender = "female" }
                );
                context.SaveChanges();

                context.Album.AddRange(
                    new Album
                    {
                        /*Id=1*/
                        Name = "Ten Summoner's Tales",
                        Released = DateTime.Parse("1993-3-9"),
                        Image= "Ten_Summoner's_Tales.png",
                        Label="A&M Records",
                        ArtistId=1
                    },
                    new Album
                    {
                        /*Id=2*/
                        Name = "Reckless",
                        Released = DateTime.Parse("1984-11-5"),
                        Image = "Reckless.jpg",
                        Label = "A&M Records",
                        ArtistId = 2
                    },
                    new Album
                    {
                        /*Id=3*/
                        Name = "Ladies & Gentlemen: The Best of George Michael",
                        Released = DateTime.Parse("1998-11-9"),
                        Image = "Ladies_&_Gentlemen.jpg",
                        Label = "Epic",
                        ArtistId = 3
                    },
                    new Album
                    {
                        /*Id=4*/
                        Name = "Greatest Hits",
                        Released = DateTime.Parse("1981-10-26"),
                        Image = "Greatest_Hits.png",
                        Label = "EMI",
                        ArtistId = 4
                    },
                    new Album
                    {
                        /*Id=5*/
                        Name = "Disintegration",
                        Released = DateTime.Parse("1989-5-2"),
                        Image = "Disintegration.jpg",
                        Label = "Fiction",
                        ArtistId = 5
                    },
                    new Album
                    {
                        /*Id=6*/
                        Name = "Slippery When Wet",
                        Released = DateTime.Parse("1986-8-18"),
                        Image = "Slippery_When_Wet.jpg",
                        Label = "Mercury",
                        ArtistId = 6
                    },
                    new Album
                    {
                        /*Id=7*/
                        Name = "Kick",
                        Released = DateTime.Parse("1987-10-12"),
                        Image = "Kick.jpg",
                        Label = "WEA",
                        ArtistId = 7
                    },
                    new Album
                    {
                        /*Id=8*/
                        Name = "Violator",
                        Released = DateTime.Parse("1990-3-19"),
                        Image = "Violator.png",
                        Label = "Mute",
                        ArtistId = 8
                    },
                    new Album
                    {
                        /*Id=9*/
                        Name = "The Queen Is Dead",
                        Released = DateTime.Parse("1986-6-16"),
                        Image = "The_Queen_is_Dead.png",
                        Label = "Rough Trade",
                        ArtistId = 9
                    },
                    new Album
                    {
                        /*Id=10*/
                        Name = "Hatful of Hollow",
                        Released = DateTime.Parse("1984-11-12"),
                        Image = "Hatful_of_Hollow.jpg",
                        Label = "Rough Trade",
                        ArtistId = 9
                    },
                    new Album
                    {
                        /*Id=11*/
                        Name = "AM",
                        Released = DateTime.Parse("2013-9-9"),
                        Image = "AM.jpg",
                        Label = "Domino",
                        ArtistId = 10
                    },
                    new Album
                    {
                        /*Id=12*/
                        Name = "Waking Up",
                        Released = DateTime.Parse("2009-11-13"),
                        Image = "Waking_Up.png",
                        Label = "Mosley",
                        ArtistId = 11
                    },
                    new Album
                    {
                        /*Id=13*/
                        Name = "Touch",
                        Released = DateTime.Parse("1983-11-14"),
                        Image = "Touch.png",
                        Label = "RCA Records",
                        ArtistId = 12
                    }
                );
                context.SaveChanges();

                context.Song.AddRange(
                    new Song
                    {
                        /*Id=1*/
                        Title = "Shape of My Heart",
                        YearReleased=1993,
                        Description= "Shape of My Heart is a song by British musician Sting, released in August 1993 as the fifth single from his fourth solo album, Ten Summoner's Tales (1993). It was used for the end credits of the 1994 film Léon, starring Jean Reno, Gary Oldman and Natalie Portman",
                        Image= "Shape_of_My_Heart.jpg",
                        ArtistId=1,
                        AlbumId=1
                    },
                    new Song
                    {
                        /*Id=2*/
                        Title = "Fields of Gold",
                        YearReleased = 1993,
                        Description = "Fields of Gold is a song written and performed by English musician Sting. It first appeared on his fourth studio album, Ten Summoner's Tales (1993).",
                        Image = "Fields_of_Gold.jpg",
                        ArtistId = 1,
                        AlbumId = 1
                    },
                    new Song
                    {
                        /*Id=3*/
                        Title = "If I Ever Lose My Faith in You",
                        YearReleased = 1993,
                        Description = "If I Ever Lose My Faith in You is a song by English singer-songwriter Sting, released on 1 February 1993 as the lead single from his fourth studio album, Ten Summoner's Tales (1993).",
                        Image = "If_I_Ever_Lose_My_Faith_In_You.jpg",
                        ArtistId = 1,
                        AlbumId = 1
                    },
                    new Song
                    {
                        /*Id=4*/
                        Title = "Heaven",
                        YearReleased = 1985,
                        Description = "Heaven is a song by the Canadian singer and songwriter Bryan Adams recorded in 1983, written by Adams and Jim Vallance.",
                        Image = "Heaven.jpg",
                        ArtistId = 2,
                        AlbumId = 2
                    },
                    new Song
                    {
                        /*Id=5*/
                        Title = "Summer of 69",
                        YearReleased = 1985,
                        Description = "Summer of '69 is a song recorded by the Canadian singer Bryan Adams from his fourth album, Reckless. It is an up-tempo rock song about a dilemma between settling down or trying to become a rock star.",
                        Image = "Summer_of_69.jpg",
                        ArtistId = 2,
                        AlbumId = 2
                    },
                    new Song
                    {
                        /*Id=6*/
                        Title = "It's Only Love",
                        YearReleased = 1985,
                        Description = "It's Only Love is a song by Canadian singer and songwriter Bryan Adams, featuring American singer Tina Turner. Released as a single on October 21, 1985, the song was nominated for a Grammy Award for Best Rock Performance by a Duo or Group with Vocal and the accompanying video won an MTV Video Music Award for Best Stage Performance.",
                        Image = "It's_Only_Love.jpg",
                        ArtistId = 2,
                        AlbumId = 2
                    },
                    new Song
                    {
                        /*Id=7*/
                        Title = "Praying for Time",
                        YearReleased = 1990,
                        Description = "Praying for Time is a song written and performed by British singer and songwriter George Michael, released on Epic Records in the United Kingdom and Columbia Records in the United States in 1990.",
                        Image = "Praying_for_Time.jpg",
                        ArtistId = 3,
                        AlbumId = 3
                    },
                    new Song
                    {
                        /*Id=8*/
                        Title = "Jesus to a Child",
                        YearReleased = 1996,
                        Description = "Jesus to a Child is a song by British singer and songwriter George Michael. It is a melancholy tribute to his late lover Anselmo Feleppa.",
                        Image = "Jesus_to_a_Child.jpg",
                        ArtistId = 3,
                        AlbumId = 3
                    },
                    new Song
                    {
                        /*Id=9*/
                        Title = "Killer Queen",
                        YearReleased = 1974,
                        Description = "Killer Queen is a song by the British rock band Queen. It is included in Queen's 1981 Greatest Hits compilation.",
                        Image = "Killer_Queen.jpg",
                        ArtistId = 4,
                        AlbumId = 4
                    },
                    new Song
                    {
                        /*Id=10*/
                        Title = "Don't Stop Me Now",
                        YearReleased = 1979,
                        Description = "Don't Stop Me Now is a song by the British rock band Queen featured on their 1978 album Jazz that was released as a single in 1979.",
                        Image = "Don't_Stop_Me_Now.jpg",
                        ArtistId = 4,
                        AlbumId = 4
                    },
                    new Song
                    {
                        /*Id=11*/
                        Title = "Lullaby",
                        YearReleased = 1989,
                        Description = "Lullaby is a song by English rock band the Cure from their eighth studio album, Disintegration (1989). Released as a single on 10 April 1989, the song is the band's highest-charting single in their home country, reaching number five on the UK Singles Chart.",
                        Image = "Lullaby.jpg",
                        ArtistId = 5,
                        AlbumId = 5
                    },
                    new Song
                    {
                        /*Id=12*/
                        Title = "Pictures of You",
                        YearReleased = 1990,
                        Description = "Pictures of You is a song by English rock band the Cure. It was released on 19 March 1990 by Fiction Records as the fourth and final single from the band's eighth studio album, Disintegration (1989).",
                        Image = "Pictures_of_You.jpg",
                        ArtistId = 5,
                        AlbumId = 5
                    },
                    new Song
                    {
                        /*Id=13*/
                        Title = "Livin' on a Prayer",
                        YearReleased = 1986,
                        Description = "Livin' on a Prayer is a song by the American rock band Bon Jovi, and is the band's second chart-topping single from their third album Slippery When Wet.",
                        Image = "Livin'_on_a_Prayer.jpg",
                        ArtistId = 6,
                        AlbumId = 6
                    },
                    new Song
                    {
                        /*Id=14*/
                        Title = "You Give Love a Bad Name",
                        YearReleased = 1986,
                        Description = "You Give Love a Bad Name is a song by American rock band Bon Jovi, released as the first single from their 1986 album Slippery When Wet.",
                        Image = "You_Give_Love_a_Bad_Name.jpg",
                        ArtistId = 6,
                        AlbumId = 6
                    },
                    new Song
                    {
                        /*Id=15*/
                        Title = "Never Tear Us Apart",
                        YearReleased = 1988,
                        Description = "Never Tear Us Apart is a song by Australian rock band INXS, released in June 1988 as the fourth single from their sixth studio album, Kick.",
                        Image = "Never_Tear_Us_Apart.jpg",
                        ArtistId = 7,
                        AlbumId = 7
                    },
                    new Song
                    {
                        /*Id=16*/
                        Title = "Need You Tonight",
                        YearReleased = 1987,
                        Description = "Need You Tonight is a song by the Australian rock band INXS, released as the first single from their 1987 album, Kick, as well as the fourth song on the album. It is the only INXS single to reach No. 1 on the Billboard Hot 100.",
                        Image = "Need_you_Tonight.jpg",
                        ArtistId = 7,
                        AlbumId = 7
                    },
                    new Song
                    {
                        /*Id=17*/
                        Title = "Policy of Truth",
                        YearReleased = 1990,
                        Description = "Policy of Truth is a song by English electronic music band Depeche Mode, released on 7 May 1990 as the third single from their seventh studio album, Violator (1990). It is the only Depeche Mode single to chart higher on the US Billboard Hot 100 chart (number 15) than on the UK Singles Chart (number 16), and it became the band's second chart-topper on the Billboard Modern Rock Tracks chart.",
                        Image = "Policy_of_Truth.jpg",
                        ArtistId = 8,
                        AlbumId = 8
                    },
                    new Song
                    {
                        /*Id=18*/
                        Title = "Personal Jesus",
                        YearReleased = 1989,
                        Description = "Personal Jesus is a song by English electronic music band Depeche Mode. It was released as the lead single from their seventh studio album, Violator (1990), in 1989. It reached No. 13 on the UK Singles Chart and No. 28 on the Billboard Hot 100.",
                        Image = "Personal_Jesus.jpg",
                        ArtistId = 8,
                        AlbumId = 8
                    },
                    new Song
                    {
                        /*Id=19*/
                        Title = "Bigmouth Strikes Again",
                        YearReleased = 1986,
                        Description = "Bigmouth Strikes Again is a 1986 song by the English rock band the Smiths from their third album The Queen Is Dead. Written by Johnny Marr and Morrissey, the song features self-deprecating lyrics that reflected Morrissey's frustrations with the music industry at the time.",
                        Image = "Bigmouth_Strikes_Again.jpg",
                        ArtistId = 9,
                        AlbumId = 9
                    },
                    new Song
                    {
                        /*Id=20*/
                        Title = "There Is a Light That Never Goes Out",
                        YearReleased = 1992,
                        Description = "There Is a Light That Never Goes Out is a song by the English rock band the Smiths, written by guitarist Johnny Marr and singer Morrissey. In 2021, it was ranked at No. 226 on Rolling Stone's Top 500 Greatest Songs of All Time.",
                        Image = "There_Is_a_Light_That_Never_Goes_Out.jpg",
                        ArtistId = 9,
                        AlbumId = 9
                    },
                    new Song
                    {
                        /*Id=21*/
                        Title = "How Soon Is Now?",
                        YearReleased = 1985,
                        Description = "How Soon Is Now? is a song by English rock band the Smiths, written by singer Morrissey and guitarist Johnny Marr.",
                        Image = "How_Soon_Is_Now.jpg",
                        ArtistId = 9,
                        AlbumId = 10
                    },
                    new Song
                    {
                        /*Id=22*/
                        Title = "This Charming Man",
                        YearReleased = 1983,
                        Description = "This Charming Man is a song by the English rock band the Smiths, written by guitarist Johnny Marr and singer Morrissey. Released as the group's second single in October 1983 on the independent record label Rough Trade, it is defined by Marr's jangle pop guitar riff and Morrissey's characteristically morose lyrics, which revolve around the recurrent Smiths themes of sexual ambiguity and lust.",
                        Image = "This_Charming_Man.png",
                        ArtistId = 9,
                        AlbumId = 10
                    },
                    new Song
                    {
                        /*Id=23*/
                        Title = "R U Mine?",
                        YearReleased = 2012,
                        Description = "R U Mine? is a song by the English indie rock band Arctic Monkeys. It features lyrics written by Arctic Monkeys frontman Alex Turner, as well as music composed by the entire band.",
                        Image = "R_U_Mine.jpg",
                        ArtistId = 10,
                        AlbumId = 11
                    },
                    new Song
                    {
                        /*Id=24*/
                        Title = "Do I Wanna Know?",
                        YearReleased = 2013,
                        Description = "Do I Wanna Know? is a song by English rock band Arctic Monkeys, with lyrics written by frontman Alex Turner. It was released on 19 June 2013 by Domino Recording Company as the second single from their fifth studio album, AM (2013).",
                        Image = "Do_I_Wanna_Know.jpg",
                        ArtistId = 10,
                        AlbumId = 11
                    },
                    new Song
                    {
                        /*Id=25*/
                        Title = "Why'd You Only Call Me When You're High?",
                        YearReleased = 2013,
                        Description = "Why'd You Only Call Me When You're High? is a song by English indie rock band Arctic Monkeys. It was released as the third single from their fifth studio album, AM, on 11 August 2013. It was written by the group's lead vocalist Alex Turner while its production was handled by James Ford.",
                        Image = "Why'd_You_Only_Call_Me_When_You're_High.jpg",
                        ArtistId = 10,
                        AlbumId = 11
                    },
                    new Song
                    {
                        /*Id=26*/
                        Title = "Arabella",
                        YearReleased = 2014,
                        Description = "Arabella is a song by English rock band Arctic Monkeys from their fifth studio album, AM (2013).",
                        Image = "Arabella.jpg",
                        ArtistId = 10,
                        AlbumId = 11
                    },
                    new Song
                    {
                        /*Id=27*/
                        Title = "All the Right Moves",
                        YearReleased = 2009,
                        Description = "All the Right Moves is the lead single by American band OneRepublic from their second studio album Waking Up (2009).",
                        Image = "All_the_Right_Moves.jpg",
                        ArtistId = 11,
                        AlbumId = 12
                    },
                    new Song
                    {
                        /*Id=28*/
                        Title = "Secrets",
                        YearReleased = 2009,
                        Description = "Secrets is the second single released from OneRepublic's second studio album, Waking Up.",
                        Image = "Secrets.jpg",
                        ArtistId = 11,
                        AlbumId = 12
                    },
                    new Song
                    {
                        /*Id=29*/
                        Title = "Who's That Girl?",
                        YearReleased = 1983,
                        Description = "Who's That Girl? is a song by British pop duo Eurythmics, released as the lead single from their third studio album, Touch (1983). It was written by band members Annie Lennox and David A. Stewart and produced by Stewart.",
                        Image = "Who's_That_Girl.jpg",
                        ArtistId = 12,
                        AlbumId = 13
                    },
                    new Song
                    {
                        /*Id=30*/
                        Title = "Here Comes the Rain Again",
                        YearReleased = 1984,
                        Description = "Here Comes the Rain Again is a 1983 song by British duo Eurythmics and the opening track from their third studio album Touch. It was written by group members Annie Lennox and David A. Stewart and produced by Stewart.",
                        Image = "Here_Comes_the_Rain_Again.jpg",
                        ArtistId = 12,
                        AlbumId = 13
                    }
                );
                context.SaveChanges();

                context.Review.AddRange(
                    new Review
                    {
                        /*Id=1*/
                        AppUser="ana@mvcrecord.com",
                        Comment="Beautiful album!",
                        Rating=10,
                        AlbumId=1
                    },
                    new Review
                    {
                        /*Id=2*/
                        AppUser = "ana@mvcrecord.com",
                        Comment = "Bryan rocks!",
                        Rating = 10,
                        AlbumId = 2
                    },
                    new Review
                    {
                        /*Id=3*/
                        AppUser = "ana@mvcrecord.com",
                        Comment = "I love the Arctic Monkeys!",
                        Rating = 10,
                        AlbumId = 11
                    },
                    new Review
                    {
                        /*Id=4*/
                        AppUser = "vesna@mvcrecord.com",
                        Comment = "Best album ever! George Michael is my favourite singer of all time!",
                        Rating = 10,
                        AlbumId = 3
                    },
                    new Review
                    {
                        /*Id=5*/
                        AppUser = "vesna@mvcrecord.com",
                        Comment = "Good record!",
                        Rating = 9,
                        AlbumId = 12
                    },
                    new Review
                    {
                        /*Id=6*/
                        AppUser = "vesna@mvcrecord.com",
                        Comment = "Michael rocks!",
                        Rating = 10,
                        AlbumId = 7
                    },
                    new Review
                    {
                        /*Id=7*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "Wow!",
                        Rating = 8,
                        AlbumId = 6
                    },
                    new Review
                    {
                        /*Id=8*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "I love Queen!",
                        Rating = 10,
                        AlbumId = 4
                    },
                    new Review
                    {
                        /*Id=9*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "Too dark for my taste!",
                        Rating = 6,
                        AlbumId = 5
                    },
                    new Review
                    {
                        /*Id=10*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "80s music at its peak!",
                        Rating = 10,
                        AlbumId = 8
                    },
                    new Review
                    {
                        /*Id=11*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "Interesting lyrics!",
                        Rating = 8,
                        AlbumId = 9
                    },
                    new Review
                    {
                        /*Id=12*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "Love this record!",
                        Rating = 9,
                        AlbumId = 10
                    },
                    new Review
                    {
                        /*Id=13*/
                        AppUser = "gorgi@mvcrecord.com",
                        Comment = "Great band and great record!",
                        Rating = 9,
                        AlbumId = 13
                    }
                );
                context.SaveChanges();

                context.UserAlbum.AddRange(
                    new UserAlbum
                    {
                        /*Id=1*/
                        AppUser= "ana@mvcrecord.com",
                        AlbumId=1
                    },
                    new UserAlbum
                    {
                        /*Id=2*/
                        AppUser = "ana@mvcrecord.com",
                        AlbumId = 2
                    },
                    new UserAlbum
                    {
                        /*Id=3*/
                        AppUser = "ana@mvcrecord.com",
                        AlbumId = 11
                    }
                );
                context.SaveChanges();

                context.Genre.AddRange(
                    new Genre
                    {
                        /*Id=1*/
                        GenreName="pop rock"
                    },
                    new Genre
                    {
                        /*Id=2*/
                        GenreName = "soft rock"
                    },
                    new Genre
                    {
                        /*Id=3*/
                        GenreName = "jazz rock"
                    },
                    new Genre
                    {
                        /*Id=4*/
                        GenreName = "hard rock"
                    },
                    new Genre
                    {
                        /*Id=5*/
                        GenreName = "arena rock"
                    },
                    new Genre
                    {
                        /*Id=6*/
                        GenreName = "pop"
                    },
                    new Genre
                    {
                        /*Id=7*/
                        GenreName = "dance-pop"
                    },
                    new Genre
                    {
                        /*Id=8*/
                        GenreName = "R&B"
                    },
                    new Genre
                    {
                        /*Id=9*/
                        GenreName = "rock"
                    },
                    new Genre
                    {
                        /*Id=10*/
                        GenreName = "gothic rock"
                    },
                    new Genre
                    {
                        /*Id=11*/
                        GenreName = "alternative rock"
                    },
                    new Genre
                    {
                        /*Id=12*/
                        GenreName = "dream pop"
                    },
                    new Genre
                    {
                        /*Id=13*/
                        GenreName = "post-punk"
                    },
                    new Genre
                    {
                        /*Id=14*/
                        GenreName = "art rock"
                    },
                    new Genre
                    {
                        /*Id=15*/
                        GenreName = "new wave"
                    },
                    new Genre
                    {
                        /*Id=16*/
                        GenreName = "glam metal"
                    },
                    new Genre
                    {
                        /*Id=17*/
                        GenreName = "pop metal"
                    },
                    new Genre
                    {
                        /*Id=18*/
                        GenreName = "funk rock"
                    },
                    new Genre
                    {
                        /*Id=19*/
                        GenreName = "synth-pop"
                    },
                    new Genre
                    {
                        /*Id=20*/
                        GenreName = "indie pop"
                    },
                    new Genre
                    {
                        /*Id=21*/
                        GenreName = "indie rock"
                    },
                    new Genre
                    {
                        /*Id=22*/
                        GenreName = "blues rock"
                    },
                    new Genre
                    {
                        /*Id=23*/
                        GenreName = "smooth jazz"
                    },
                    new Genre
                    {
                        /*Id=24*/
                        GenreName = "jazz"
                    }
                );
                context.SaveChanges();

                context.AlbumGenre.AddRange(
                    new AlbumGenre
                    {
                        /*Id=1*/
                        AlbumId=1,
                        GenreId=1
                    },
                    new AlbumGenre
                    {
                        /*Id=2*/
                        AlbumId = 1,
                        GenreId = 2
                    },
                    new AlbumGenre
                    {
                        /*Id=3*/
                        AlbumId = 1,
                        GenreId = 3
                    },
                    new AlbumGenre
                    {
                        /*Id=4*/
                        AlbumId = 2,
                        GenreId = 4
                    },
                    new AlbumGenre
                    {
                        /*Id=5*/
                        AlbumId = 2,
                        GenreId = 5
                    },
                    new AlbumGenre
                    {
                        /*Id=6*/
                        AlbumId = 3,
                        GenreId = 1
                    },
                    new AlbumGenre
                    {
                        /*Id=7*/
                        AlbumId = 3,
                        GenreId = 6
                    },
                    new AlbumGenre
                    {
                        /*Id=8*/
                        AlbumId = 3,
                        GenreId = 7
                    },
                    new AlbumGenre
                    {
                        /*Id=9*/
                        AlbumId = 3,
                        GenreId = 8
                    },
                    new AlbumGenre
                    {
                        /*Id=10*/
                        AlbumId = 4,
                        GenreId = 9
                    },
                    new AlbumGenre
                    {
                        /*Id=11*/
                        AlbumId = 5,
                        GenreId = 10
                    },
                    new AlbumGenre
                    {
                        /*Id=12*/
                        AlbumId = 5,
                        GenreId = 11
                    },
                    new AlbumGenre
                    {
                        /*Id=13*/
                        AlbumId = 5,
                        GenreId = 12
                    },
                    new AlbumGenre
                    {
                        /*Id=14*/
                        AlbumId = 5,
                        GenreId = 13
                    },
                    new AlbumGenre
                    {
                        /*Id=15*/
                        AlbumId = 5,
                        GenreId = 14
                    },
                    new AlbumGenre
                    {
                        /*Id=16*/
                        AlbumId = 5,
                        GenreId = 15
                    },
                    new AlbumGenre
                    {
                        /*Id=17*/
                        AlbumId = 6,
                        GenreId = 1
                    },
                    new AlbumGenre
                    {
                        /*Id=18*/
                        AlbumId = 6,
                        GenreId = 4
                    },
                    new AlbumGenre
                    {
                        /*Id=19*/
                        AlbumId = 6,
                        GenreId = 16
                    },
                    new AlbumGenre
                    {
                        /*Id=20*/
                        AlbumId = 6,
                        GenreId = 17
                    },
                    new AlbumGenre
                    {
                        /*Id=21*/
                        AlbumId = 7,
                        GenreId = 1
                    },
                    new AlbumGenre
                    {
                        /*Id=22*/
                        AlbumId = 7,
                        GenreId = 4
                    },
                    new AlbumGenre
                    {
                        /*Id=23*/
                        AlbumId = 7,
                        GenreId = 15
                    },
                    new AlbumGenre
                    {
                        /*Id=24*/
                        AlbumId = 7,
                        GenreId = 18
                    },
                    new AlbumGenre
                    {
                        /*Id=25*/
                        AlbumId = 8,
                        GenreId = 11
                    },
                    new AlbumGenre
                    {
                        /*Id=26*/
                        AlbumId = 8,
                        GenreId = 19
                    },
                    new AlbumGenre
                    {
                        /*Id=27*/
                        AlbumId = 9,
                        GenreId = 11
                    },
                    new AlbumGenre
                    {
                        /*Id=28*/
                        AlbumId = 9,
                        GenreId = 13
                    },
                    new AlbumGenre
                    {
                        /*Id=29*/
                        AlbumId = 9,
                        GenreId = 20
                    },
                    new AlbumGenre
                    {
                        /*Id=30*/
                        AlbumId = 9,
                        GenreId = 21
                    },
                    new AlbumGenre
                    {
                        /*Id=31*/
                        AlbumId = 10,
                        GenreId = 11
                    },
                    new AlbumGenre
                    {
                        /*Id=32*/
                        AlbumId = 10,
                        GenreId = 13
                    },
                    new AlbumGenre
                    {
                        /*Id=33*/
                        AlbumId = 10,
                        GenreId = 20
                    },
                    new AlbumGenre
                    {
                        /*Id=34*/
                        AlbumId = 11,
                        GenreId = 4
                    },
                    new AlbumGenre
                    {
                        /*Id=35*/
                        AlbumId = 11,
                        GenreId = 21
                    },
                    new AlbumGenre
                    {
                        /*Id=36*/
                        AlbumId = 11,
                        GenreId = 22
                    },
                    new AlbumGenre
                    {
                        /*Id=37*/
                        AlbumId = 12,
                        GenreId = 1
                    },
                    new AlbumGenre
                    {
                        /*Id=38*/
                        AlbumId = 13,
                        GenreId = 15
                    },
                    new AlbumGenre
                    {
                        /*Id=39*/
                        AlbumId = 13,
                        GenreId = 19
                    }
                );
                context.SaveChanges();

                context.SongGenre.AddRange(
                    new SongGenre
                    {
                        /*Id=1*/
                        SongId= 1,
                        GenreId= 9
                    },
                    new SongGenre
                    {
                        /*Id=2*/
                        SongId = 2,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=3*/
                        SongId = 2,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=4*/
                        SongId = 2,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=5*/
                        SongId = 3,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=6*/
                        SongId = 3,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=7*/
                        SongId = 4,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=8*/
                        SongId = 5,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=9*/
                        SongId = 5,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=10*/
                        SongId = 6,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=11*/
                        SongId = 6,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=12*/
                        SongId = 7,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=13*/
                        SongId = 8,
                        GenreId = 7
                    },
                    new SongGenre
                    {
                        /*Id=14*/
                        SongId = 8,
                        GenreId = 23
                    },
                    new SongGenre
                    {
                        /*Id=15*/
                        SongId = 9,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=16*/
                        SongId = 10,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=17*/
                        SongId = 10,
                        GenreId = 4
                    },
                    new SongGenre
                    {
                        /*Id=18*/
                        SongId = 10,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=19*/
                        SongId = 11,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=20*/
                        SongId = 12,
                        GenreId = 10
                    },
                    new SongGenre
                    {
                        /*Id=21*/
                        SongId = 12,
                        GenreId = 19
                    },
                    new SongGenre
                    {
                        /*Id=22*/
                        SongId = 13,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=23*/
                        SongId = 13,
                        GenreId = 16
                    },
                    new SongGenre
                    {
                        /*Id=24*/
                        SongId = 13,
                        GenreId = 5
                    },
                    new SongGenre
                    {
                        /*Id=25*/
                        SongId = 14,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=26*/
                        SongId = 14,
                        GenreId = 4
                    },
                    new SongGenre
                    {
                        /*Id=27*/
                        SongId = 14,
                        GenreId = 16
                    },
                    new SongGenre
                    {
                        /*Id=28*/
                        SongId = 15,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=29*/
                        SongId = 15,
                        GenreId = 24
                    },
                    new SongGenre
                    {
                        /*Id=30*/
                        SongId = 15,
                        GenreId = 22
                    },
                    new SongGenre
                    {
                        /*Id=31*/
                        SongId = 15,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=32*/
                        SongId = 15,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=33*/
                        SongId = 16,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=34*/
                        SongId = 16,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=35*/
                        SongId = 17,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=36*/
                        SongId = 17,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=37*/
                        SongId = 17,
                        GenreId = 11
                    },
                    new SongGenre
                    {
                        /*Id=38*/
                        SongId = 17,
                        GenreId = 19
                    },
                    new SongGenre
                    {
                        /*Id=39*/
                        SongId = 18,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=40*/
                        SongId = 18,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=41*/
                        SongId = 18,
                        GenreId = 11
                    },
                    new SongGenre
                    {
                        /*Id=42*/
                        SongId = 18,
                        GenreId = 22
                    },
                    new SongGenre
                    {
                        /*Id=43*/
                        SongId = 19,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=44*/
                        SongId = 19,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=45*/
                        SongId = 19,
                        GenreId = 13
                    },
                    new SongGenre
                    {
                        /*Id=46*/
                        SongId = 20,
                        GenreId = 11
                    },
                    new SongGenre
                    {
                        /*Id=47*/
                        SongId = 21,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=48*/
                        SongId = 21,
                        GenreId = 11
                    },
                    new SongGenre
                    {
                        /*Id=49*/
                        SongId = 22,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=50*/
                        SongId = 23,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=51*/
                        SongId = 24,
                        GenreId = 22
                    },
                    new SongGenre
                    {
                        /*Id=52*/
                        SongId = 25,
                        GenreId = 8
                    },
                    new SongGenre
                    {
                        /*Id=53*/
                        SongId = 25,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=54*/
                        SongId = 26,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=55*/
                        SongId = 27,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=56*/
                        SongId = 27,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=57*/
                        SongId = 28,
                        GenreId = 1
                    },
                    new SongGenre
                    {
                        /*Id=58*/
                        SongId = 28,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=59*/
                        SongId = 28,
                        GenreId = 9
                    },
                    new SongGenre
                    {
                        /*Id=60*/
                        SongId = 29,
                        GenreId = 6
                    },
                    new SongGenre
                    {
                        /*Id=61*/
                        SongId = 30,
                        GenreId = 6
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
