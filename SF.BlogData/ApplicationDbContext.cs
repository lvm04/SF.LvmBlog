using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using SF.BlogData.Models;

namespace SF.BlogData
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());

            // Имена таблиц в единственном числе
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
            }
        }

        public class RoleConfiguration : IEntityTypeConfiguration<Role>
        {
            public void Configure(EntityTypeBuilder<Role> builder)
            {
                // Таблицы User, Role и отношение между ними "многие ко многим"
                var role1 = new Role { Id = 1, Name = "admin" };
                var role2 = new Role { Id = 2, Name = "user" };
                var role3 = new Role { Id = 3, Name = "moderator" };
                builder.HasData(role1, role2, role3);
            }
        }

        public class UserConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.HasData(
                    new User { Id = 1, Login = "admin", Name = "Администратор", Password = "123", Email = "admin@mail.ru" },
                    new User { Id = 2, Login = "user", Name = "Пользователь", Password = "123", Email = "user@mail.ru" },
                    new User { Id = 3, Login = "moder", Name = "Модератор", Password = "123", Email = "moder@mail.ru" },
                    new User { Id = 4, Login = "expert", Name = "Опытный", Password = "123", Email = "expert@mail.ru" }
                );

                builder.HasMany(u => u.Roles)
                        .WithMany(r => r.Users)
                        .UsingEntity(j => j.HasData(new { UsersId = 1, RolesId = 1 },
                                                    new { UsersId = 2, RolesId = 2 },
                                                    new { UsersId = 3, RolesId = 3 },
                                                    new { UsersId = 4, RolesId = 2 },
                                                    new { UsersId = 4, RolesId = 3 }
                        ));

                builder.HasIndex(u => u.Login).IsUnique().HasDatabaseName("Login_INDEX");
            }
        }

        public class TagConfiguration : IEntityTypeConfiguration<Tag>
        {
            public void Configure(EntityTypeBuilder<Tag> builder)
            {
                var tag1 = new Tag { Id = 1, Name = "История" };
                var tag2 = new Tag { Id = 2, Name = "Музыка" };
                var tag3 = new Tag { Id = 3, Name = "География" };
                var tag4 = new Tag { Id = 4, Name = "Литература" };
                builder.HasData(tag1, tag2, tag3, tag4);
            }
        }

        public class ArticleConfiguration : IEntityTypeConfiguration<Article>
        {
            public void Configure(EntityTypeBuilder<Article> builder)
            {
                var article1 = new Article
                {
                    Id = 1,
                    Title = "Лимпопо",
                    AuthorId = 2,
                    Text = "Река в Южной Африке на территории ЮАР, Ботсваны, Зимбабве и Мозамбика. " +
                    "Этимология неизвестна. Буры назвали её \"крокодиловой рекой\" из-за обилия крокодилов." +
                    "Начинается южнее Претории в горах Витватерсранд (1800 м), " +
                    "затем пересекает горы Могали и принимает приток Марико. Пройдя 1750 км и приняв в себя множество притоков, " +
                    "впадает в Индийский океан к северу от залива Делагоа. Образуется при слиянии рек Марико и Крокодил. " +
                    "На реке имеется несколько порогов, она течёт зигзагами и служит природной границей между несколькими странами. " +
                    "Длина реки составляет 1750 километров, а площадь водосборного бассейна — 440 000 квадратных километров. " +
                    "Крупнейшим правым притоком Лимпопо является Улифантс. К северо-востоку от реки находится национальный парк Крюгера. ",

                };
                var article2 = new Article
                {
                    Id = 2,
                    Title = "Смутное время",
                    AuthorId = 4,
                    Text = "Период в истории России с 1598 года по 1613 год (согласно некоторым точкам зрения по 1618 год), " +
                            "ознаменованный стихийными бедствиями, сопровождающийся многочисленными случаями самозванства и " +
                            "внешней интервенцией, гражданской, русско-польской и русско-шведской войнами, тяжелейшими " +
                            "государственно-политическим и социально-экономическим кризисами."
                };
                var article3 = new Article
                {
                    Id = 3,
                    Title = "Моцарт",
                    AuthorId = 4,
                    Text = "Австрийский композитор и музыкант-виртуоз. Один из самых популярных классических композиторов, " + 
                            "Моцарт оказал большое влияние на мировую музыкальную культуру. По свидетельству современников, " + 
                            "Моцарт обладал феноменальным музыкальным слухом, памятью и способностью к импровизации. " + 
                            "Самый молодой член Болонской филармонической академии (с 1770 года) за всю её историю, " + 
                            "а также самый молодой кавалер ордена Золотой шпоры (1770)."
                };
                var article4 = new Article
                {
                    Id = 4,
                    Title = "Ричард II (пьеса)",
                    AuthorId = 4,
                    Text = "Историческая хроника Уильяма Шекспира (1595), охватывает события 1399—1400 гг.; в её центре " +
                            "— низложение короля Ричарда II и захват власти его двоюродным братом Генрихом Болингброком " +
                            "— основателем дома Ланкастеров Генрихом IV, а затем убийство пленного Ричарда. " +
                            "В ряде прижизненных изданий названа трагедией."
                };
                var article5 = new Article
                {
                    Id = 5,
                    Title = "Гарри Поттер",
                    AuthorId = 2,
                    Text = "Литературный персонаж, главный герой серии романов английской писательницы Джоан Роулинг. " +
                            "На одиннадцатый день рождения, Гарри узнаёт, что является волшебником и ему уготовано место " + 
                            "в школе волшебства \"Хогвартс\", в которой он будет практиковать магию под руководством директора " + 
                            "Альбуса Дамблдора и других школьных профессоров.Также Гарри обнаруживает, что он уже известен во " + 
                            "всём магическом сообществе романа, и что его судьба связана с судьбой Волан - де - Морта, " +
                            "опасного тёмного мага, убившего среди прочих и его родителей — Лили и Джеймса Поттер."
                };
                var article6 = new Article
                {
                    Id = 6,
                    Title = "Джомолунгма (Эверест)",
                    AuthorId = 3,
                    Text = "Высочайшая вершина Земли (8848,86 м). Вершина находится в Гималаях в хребте Махалангур-Химал, " + 
                            "по которому проходит граница Непала и Тибетского автономного района (Китай). Эверест имеет форму " + 
                            "трёхгранной пирамиды, южный склон более крутой. На южном склоне и рёбрах снег и фирн не удерживаются, " +
                            "вследствие чего они обнажены. Высота Северо - восточного плеча — 8393 м. Высота от подножия до вершины — " + 
                            "около 3550 м. Вершина состоит в основном из осадочных отложений."
                };

                builder.HasData(article1, article2, article3, article4, article5, article6);

                builder.HasMany(u => u.Tags)
                        .WithMany(r => r.Articles)
                        .UsingEntity(j => j.HasData(
                            new { ArticlesId = 1, TagsId = 3 },
                            new { ArticlesId = 2, TagsId = 1 },
                            new { ArticlesId = 3, TagsId = 2 },
                            new { ArticlesId = 4, TagsId = 1 },
                            new { ArticlesId = 4, TagsId = 4 },
                            new { ArticlesId = 5, TagsId = 4 },
                            new { ArticlesId = 6, TagsId = 3 }
                        ));

                builder.Property(u => u.TimeStamp).HasDefaultValueSql("DATETIME('now', 'localtime')");
                // Внешний ключ на автора
                builder.HasOne(a => a.Author)
                        .WithMany(u => u.Articles)
                        .HasForeignKey(a => a.AuthorId)
                        .OnDelete(DeleteBehavior.SetNull);
            }
        }

        public class CommentConfiguration : IEntityTypeConfiguration<Comment>
        {
            public void Configure(EntityTypeBuilder<Comment> builder)
            {
                var comment1 = new Comment { Id = 1, ArticleId = 1, AuthorId = 2, 
                    Text = "Лимпопо судоходна на 160 км, от места, где в неё, в районе 32° в. д., впадает Мвензи (также именуемая Нуанетси)."
                };
                var comment2 = new Comment { Id = 2, ArticleId = 1, AuthorId = 3, 
                    Text = "В русской детской литературе Лимпопо упоминается в сказке Корнея Ивановича Чуковского \"Айболит\"."
                };
                var comment3 = new Comment { Id = 3, ArticleId = 2, AuthorId = 4, 
                    Text = "Наследник Ивана Грозного Фёдор Иванович (с 1584) правил до 1598 года, а младший сын, царевич Дмитрий, погиб при " + 
                            "таинственных обстоятельствах в Угличе в 1591 году. С их смертью правящая династия пресеклась, на сцену выдвинулись " + 
                            "боярские роды — Захарьины (Романовы), Годуновы. В 1598 году на трон был возведён Борис Годунов."
                };
                var comment4 = new Comment { Id = 4, ArticleId = 2, AuthorId = 3, 
                    Text = "Три года, с 1601 по 1603, были неурожайными, даже в летние месяцы не прекращались заморозки, а в сентябре выпадал снег."
                };
                var comment5 = new Comment { Id = 5, ArticleId = 3, AuthorId = 4, 
                    Text = "В отличие от многих композиторов XVIII века, Моцарт не просто работал во всех музыкальных формах своего времени, " + 
                            "но и добился в них большого успеха. Многие из его сочинений признаны шедеврами симфонической, концертной, камерной, " + 
                            "оперной и хоровой музыки."
                };
                var comment6 = new Comment { Id = 6, ArticleId = 3, AuthorId = 1, 
                    Text = "Многие из его сочинений признаны шедеврами симфонической, концертной, камерной, оперной и хоровой музыки. " + 
                            "Наряду с Гайдном и Бетховеном принадлежит к наиболее значительным представителям Венской классической школы."
                };
                var comment7 = new Comment { Id = 7, ArticleId = 2, AuthorId = 2, 
                    Text = "С началом Смуты распространились слухи о том, что законный царевич Дмитрий жив. Из этого следовало, что правление Бориса " + 
                    "Годунова незаконно. Самозванец Лжедмитрий, объявивший западнорусскому князю Адаму Вишневецкому о своём царском происхождении, " + 
                    "вошёл в тесные отношения с польским магнатом, воеводой сандомирским Ежи Мнишеком и папским нунцием Рангони."
                };
                var comment8 = new Comment { Id = 8, ArticleId = 4, AuthorId = 3, 
                    Text = "Ричард оставил заметный след в истории Англии и её культуре, а его свержение стало первым шагом к серии феодальных " + 
                            "междоусобиц во второй половине XV века, известных как Война Алой и Белой розы."
                };
                var comment9 = new Comment { Id = 9, ArticleId = 4, AuthorId = 2, 
                    Text = "В изображении Шекспира Ричард II — неудачный, слабый, но уверенный в божественности своей власти правитель, поручающий " + 
                            "власть алчным фаворитам."
                };
                var comment10 = new Comment { Id = 10, ArticleId = 5, AuthorId = 4, 
                    Text = "Гарри Поттер был защищён мощной магией, благодаря его матери, сумевшей применить древнее заклинание защиты во время нападения Тёмного Лорда."
                };
                var comment11 = new Comment { Id = 11, ArticleId = 5, AuthorId = 2, 
                    Text = "Гарри Поттер обладает большими способностями в области Защиты от Тёмных Искусств. Уже на третьем курсе он научился вызывать Патронуса."
                };
                var comment12 = new Comment { Id = 12, ArticleId = 5, AuthorId = 3, 
                    Text = "Гарри вырос в тяжёлой эмоциональной обстановке, но это не сказалось плохо на его характере. Помимо доброты и отваги, Гарри " + 
                            "наделён такими качествами, как милосердие, сострадание и способность к самопожертвованию."
                };
                var comment13 = new Comment { Id = 13, ArticleId = 6, AuthorId = 1, 
                    Text = "Средняя дневная температура на вершине Джомолунгмы в июле — порядка −19 °С, в январе — −36 °C (и может понижаться до −60 °C)" };
                var comment14 = new Comment { Id = 14, ArticleId = 5, AuthorId = 4, 
                    Text = "У Гарри обнаружился выдающийся талант к полётам на метле. Он был принят ловцом в команду Гриффиндора по квиддичу, " + 
                            "став самым молодым ловцом за последние 100 лет. В дальнейшем Гарри оказался отличным игроком"
                };
                var comment15 = new Comment { Id = 15, ArticleId = 2, AuthorId = 2, 
                    Text = "Поражение войск Дмитрия Шуйского от поляков под Клушиным, а также повторное появление Лжедмитрия II под Москвой, окончательно " + 
                            "подорвало шаткий авторитет \"боярского царя\", и в этих условиях в Москве произошёл переворот."
                };

                builder.HasData(comment1, comment2, comment3, comment4, comment5,
                                comment6, comment7, comment8, comment9, comment10,
                                comment11, comment12, comment13, comment14, comment15);

                builder.Property(u => u.TimeStamp).HasDefaultValueSql("DATETIME('now', 'localtime')");
            }
        }

    }


    // dotnet-ef migrations add [name]
    // dotnet-ef database update
}