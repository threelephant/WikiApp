using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WikiApp.Models
{
    public partial class WikiContext : DbContext
    {
        public WikiContext()
        {
        }

        public WikiContext(DbContextOptions<WikiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ИсторияПравокСтатьи> ИсторияПравокСтатьиs { get; set; }
        public virtual DbSet<МодераторСтатьи> МодераторСтатьиs { get; set; }
        public virtual DbSet<Пользователи> Пользователиs { get; set; }
        public virtual DbSet<Роль> Рольs { get; set; }
        public virtual DbSet<Слова> Словаs { get; set; }
        public virtual DbSet<СтатусыЗаявокНаМодерацию> СтатусыЗаявокНаМодерациюs { get; set; }
        public virtual DbSet<СтатусыПравокСтатьи> СтатусыПравокСтатьиs { get; set; }
        public virtual DbSet<Статьи> Статьиs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-C5GGM6D;Initial Catalog=Wiki;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<ИсторияПравокСтатьи>(entity =>
            {
                entity.HasKey(e => e.IdПравки)
                    .HasName("Unique_Identifier1");

                entity.ToTable("История правок статьи");

                entity.HasIndex(e => new { e.ЛогинМодератора, e.IdМодерируемойСтатьи }, "IX_Одобряет");

                entity.HasIndex(e => e.Логин, "IX_Подает заявку на статью");

                entity.HasIndex(e => e.FkIdПравки, "IX_Предыдущая правка");

                entity.HasIndex(e => e.IdСтатьи, "IX_Содержит историю");

                entity.HasIndex(e => e.IdСтатуса, "IX_Статус заявки на статью");

                entity.Property(e => e.IdПравки)
                    .HasColumnName("ID правки")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.FkIdПравки).HasColumnName("FK_ID правки");

                entity.Property(e => e.IdМодерируемойСтатьи).HasColumnName("ID модерируемой статьи");

                entity.Property(e => e.IdСтатуса).HasColumnName("ID статуса");

                entity.Property(e => e.IdСтатьи).HasColumnName("ID статьи");

                entity.Property(e => e.ДатаЗаявки)
                    .HasColumnType("datetime")
                    .HasColumnName("Дата заявки");

                entity.Property(e => e.ДатаРассмотрения)
                    .HasColumnType("datetime")
                    .HasColumnName("Дата рассмотрения");

                entity.Property(e => e.Логин)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ЛогинМодератора)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Логин модератора");

                entity.Property(e => e.Текст)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.FkIdПравкиNavigation)
                    .WithMany(p => p.InverseFkIdПравкиNavigation)
                    .HasForeignKey(d => d.FkIdПравки)
                    .HasConstraintName("Предыдущая правка");

                entity.HasOne(d => d.IdСтатусаNavigation)
                    .WithMany(p => p.ИсторияПравокСтатьиs)
                    .HasForeignKey(d => d.IdСтатуса)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Статус заявки на статью");

                entity.HasOne(d => d.IdСтатьиNavigation)
                    .WithMany(p => p.ИсторияПравокСтатьиs)
                    .HasForeignKey(d => d.IdСтатьи)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Содержит историю");

                entity.HasOne(d => d.ЛогинNavigation)
                    .WithMany(p => p.ИсторияПравокСтатьиs)
                    .HasForeignKey(d => d.Логин)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Подает заявку на статью");

                entity.HasOne(d => d.МодераторСтатьи)
                    .WithMany(p => p.ИсторияПравокСтатьиs)
                    .HasForeignKey(d => new { d.ЛогинМодератора, d.IdМодерируемойСтатьи })
                    .HasConstraintName("Одобряет");
            });

            modelBuilder.Entity<МодераторСтатьи>(entity =>
            {
                entity.HasKey(e => new { e.Логин, e.IdСтатьи })
                    .HasName("Unique_Identifier5");

                entity.ToTable("Модератор статьи");

                entity.HasIndex(e => e.ЛогинАдминистратора, "IX_Рассматривает");

                entity.HasIndex(e => e.IdСтатуса, "IX_Статусы заявок");

                entity.Property(e => e.Логин)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IdСтатьи).HasColumnName("ID статьи");

                entity.Property(e => e.IdСтатуса).HasColumnName("ID статуса");

                entity.Property(e => e.ДатаЗаявки)
                    .HasColumnType("datetime")
                    .HasColumnName("Дата заявки");

                entity.Property(e => e.ДатаРассмотрения)
                    .HasColumnType("datetime")
                    .HasColumnName("Дата рассмотрения");

                entity.Property(e => e.ЛогинАдминистратора)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Логин администратора");

                entity.HasOne(d => d.IdСтатусаNavigation)
                    .WithMany(p => p.МодераторСтатьиs)
                    .HasForeignKey(d => d.IdСтатуса)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Статусы заявок");

                entity.HasOne(d => d.IdСтатьиNavigation)
                    .WithMany(p => p.МодераторСтатьиs)
                    .HasForeignKey(d => d.IdСтатьи)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Модерируется");

                entity.HasOne(d => d.ЛогинNavigation)
                    .WithMany(p => p.МодераторСтатьиЛогинNavigations)
                    .HasForeignKey(d => d.Логин)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Является");

                entity.HasOne(d => d.ЛогинАдминистратораNavigation)
                    .WithMany(p => p.МодераторСтатьиЛогинАдминистратораNavigations)
                    .HasForeignKey(d => d.ЛогинАдминистратора)
                    .HasConstraintName("Рассматривает");
            });

            modelBuilder.Entity<Пользователи>(entity =>
            {
                entity.HasKey(e => e.Логин)
                    .HasName("Unique_Identifier4");

                entity.ToTable("Пользователи");

                entity.HasIndex(e => e.IdРоли, "IX_Принадлежит");

                entity.Property(e => e.Логин)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IdРоли).HasColumnName("ID роли");

                entity.Property(e => e.ДатаРегистрации)
                    .HasColumnType("datetime")
                    .HasColumnName("Дата регистрации");

                entity.Property(e => e.Пароль)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdРолиNavigation)
                    .WithMany(p => p.Пользователиs)
                    .HasForeignKey(d => d.IdРоли)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Принадлежит");
            });

            modelBuilder.Entity<Роль>(entity =>
            {
                entity.HasKey(e => e.IdРоли)
                    .HasName("Unique_Identifier15");

                entity.ToTable("Роль");

                entity.Property(e => e.IdРоли)
                    .HasColumnName("ID роли")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.НаименованиеРоли)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Наименование роли");
            });

            modelBuilder.Entity<Слова>(entity =>
            {
                entity.HasKey(e => e.IdСлова)
                    .HasName("Unique_Identifier7");

                entity.ToTable("Слова");

                entity.Property(e => e.IdСлова)
                    .HasColumnName("ID слова")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Слово)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<СтатусыЗаявокНаМодерацию>(entity =>
            {
                entity.HasKey(e => e.IdСтатуса)
                    .HasName("Unique_Identifier13");

                entity.ToTable("Статусы заявок на модерацию");

                entity.Property(e => e.IdСтатуса)
                    .HasColumnName("ID статуса")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Наименование)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<СтатусыПравокСтатьи>(entity =>
            {
                entity.HasKey(e => e.IdСтатуса)
                    .HasName("Unique_Identifier9");

                entity.ToTable("Статусы правок статьи");

                entity.Property(e => e.IdСтатуса)
                    .HasColumnName("ID статуса")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Наименование)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Статьи>(entity =>
            {
                entity.HasKey(e => e.IdСтатьи)
                    .HasName("Unique_Identifier3");

                entity.ToTable("Статьи");

                entity.HasIndex(e => e.IdСлова, "IX_Содержит");

                entity.Property(e => e.IdСтатьи)
                    .HasColumnName("ID статьи")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IdСлова).HasColumnName("ID слова");

                entity.HasOne(d => d.IdСловаNavigation)
                    .WithMany(p => p.Статьиs)
                    .HasForeignKey(d => d.IdСлова)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Содержит");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
