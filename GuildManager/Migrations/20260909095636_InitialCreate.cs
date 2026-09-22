using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MonProjet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_classes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_classes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    slot = table.Column<string>(type: "text", nullable: false),
                    rarity = table.Column<string>(type: "text", nullable: false),
                    bonus_attack = table.Column<int>(type: "integer", nullable: false),
                    bonus_defence = table.Column<int>(type: "integer", nullable: false),
                    bonus_magic = table.Column<int>(type: "integer", nullable: false),
                    bonus_hp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quest_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quest_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    salt = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    class_id = table.Column<int>(type: "integer", nullable: false),
                    can_switch_to_healer = table.Column<bool>(type: "boolean", nullable: false),
                    base_hp = table.Column<int>(type: "integer", nullable: false),
                    base_attack = table.Column<int>(type: "integer", nullable: false),
                    base_defence = table.Column<int>(type: "integer", nullable: false),
                    base_magic = table.Column<int>(type: "integer", nullable: false),
                    recruitment_in_level = table.Column<int>(type: "integer", nullable: false),
                    base_recruitment_cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_templates", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_templates_character_classes_class_id",
                        column: x => x.class_id,
                        principalTable: "character_classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quest_type_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    required_guild_level = table.Column<int>(type: "integer", nullable: false),
                    story_step = table.Column<int>(type: "integer", nullable: false),
                    base_success_rate = table.Column<int>(type: "integer", nullable: false),
                    min_party_size = table.Column<int>(type: "integer", nullable: false),
                    max_party_size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quests", x => x.id);
                    table.ForeignKey(
                        name: "fk_quests_quest_types_quest_type_id",
                        column: x => x.quest_type_id,
                        principalTable: "quest_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    founder_user_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    gold = table.Column<int>(type: "integer", nullable: false),
                    food = table.Column<int>(type: "integer", nullable: false),
                    reputation_total = table.Column<int>(type: "integer", nullable: false),
                    is_defeated = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guilds", x => x.id);
                    table.ForeignKey(
                        name: "fk_guilds_users_founder_user_id",
                        column: x => x.founder_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip_address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quest_maluses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quest_id = table.Column<int>(type: "integer", nullable: false),
                    malus_type = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quest_maluses", x => x.id);
                    table.ForeignKey(
                        name: "fk_quest_maluses_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quest_rewards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    quest_id = table.Column<int>(type: "integer", nullable: false),
                    reward_type = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quest_rewards", x => new { x.id, x.quest_id });
                    table.ForeignKey(
                        name: "fk_quest_rewards_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_quest_rewards_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "guild_inventories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_inventories", x => x.id);
                    table.ForeignKey(
                        name: "fk_guild_inventories_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_guild_inventories_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "guild_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    guild_id = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    xp = table.Column<int>(type: "integer", nullable: false),
                    reputation = table.Column<int>(type: "integer", nullable: false),
                    is_founder = table.Column<bool>(type: "boolean", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_guild_members_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_guild_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "guild_member_characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_member_id = table.Column<int>(type: "integer", nullable: false),
                    character_template_id = table.Column<int>(type: "integer", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    xp = table.Column<int>(type: "integer", nullable: false),
                    current_hp = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    is_healer_mode = table.Column<bool>(type: "boolean", nullable: false),
                    recruited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_member_characters", x => x.id);
                    table.ForeignKey(
                        name: "fk_guild_member_characters_character_templates_character_templ",
                        column: x => x.character_template_id,
                        principalTable: "character_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_guild_member_characters_guild_members_guild_member_id",
                        column: x => x.guild_member_id,
                        principalTable: "guild_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quest_attempts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quest_id = table.Column<int>(type: "integer", nullable: false),
                    guild_member_id = table.Column<int>(type: "integer", nullable: false),
                    turn_number = table.Column<int>(type: "integer", nullable: false),
                    computed_success_rate = table.Column<int>(type: "integer", nullable: false),
                    is_success = table.Column<bool>(type: "boolean", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quest_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_quest_attempts_guild_members_guild_member_id",
                        column: x => x.guild_member_id,
                        principalTable: "guild_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_quest_attempts_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recruitment_offers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    guild_member_id = table.Column<int>(type: "integer", nullable: false),
                    character_template_id = table.Column<int>(type: "integer", nullable: false),
                    turn_number = table.Column<int>(type: "integer", nullable: false),
                    gold_cost = table.Column<int>(type: "integer", nullable: false),
                    is_purchased = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recruitment_offers", x => new { x.id, x.guild_member_id });
                    table.ForeignKey(
                        name: "fk_recruitment_offers_character_templates_character_template_id",
                        column: x => x.character_template_id,
                        principalTable: "character_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recruitment_offers_guild_members_guild_member_id",
                        column: x => x.guild_member_id,
                        principalTable: "guild_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "turns",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_member_id = table.Column<int>(type: "integer", nullable: false),
                    turn_number = table.Column<int>(type: "integer", nullable: false),
                    guild_gold_after = table.Column<int>(type: "integer", nullable: false),
                    guild_food_after = table.Column<int>(type: "integer", nullable: false),
                    played_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_turns", x => x.id);
                    table.ForeignKey(
                        name: "fk_turns_guild_members_guild_member_id",
                        column: x => x.guild_member_id,
                        principalTable: "guild_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "character_equipments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_member_character_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    slot = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_equipments", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_equipments_guild_member_characters_guild_member_c",
                        column: x => x.guild_member_character_id,
                        principalTable: "guild_member_characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_character_equipments_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quest_attempt_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quest_attempt_id = table.Column<int>(type: "integer", nullable: false),
                    guild_member_character_id = table.Column<int>(type: "integer", nullable: false),
                    survived = table.Column<bool>(type: "boolean", nullable: false),
                    hp_lost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quest_attempt_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_quest_attempt_members_guild_member_characters_guild_member_",
                        column: x => x.guild_member_character_id,
                        principalTable: "guild_member_characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_quest_attempt_members_quest_attempts_quest_attempt_id",
                        column: x => x.quest_attempt_id,
                        principalTable: "quest_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quest_attempt_reward_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quest_attempt_id = table.Column<int>(type: "integer", nullable: false),
                    reward_type = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quest_attempt_reward_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_quest_attempt_reward_logs_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_quest_attempt_reward_logs_quest_attempts_quest_attempt_id",
                        column: x => x.quest_attempt_id,
                        principalTable: "quest_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_equipments_guild_member_character_id",
                table: "character_equipments",
                column: "guild_member_character_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_equipments_item_id",
                table: "character_equipments",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_templates_class_id",
                table: "character_templates",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_inventories_guild_id",
                table: "guild_inventories",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_inventories_item_id",
                table: "guild_inventories",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_member_characters_character_template_id",
                table: "guild_member_characters",
                column: "character_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_member_characters_guild_member_id",
                table: "guild_member_characters",
                column: "guild_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_members_guild_id",
                table: "guild_members",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_members_user_id",
                table: "guild_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_guilds_founder_user_id",
                table: "guilds",
                column: "founder_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_attempt_members_guild_member_character_id",
                table: "quest_attempt_members",
                column: "guild_member_character_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_attempt_members_quest_attempt_id",
                table: "quest_attempt_members",
                column: "quest_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_attempt_reward_logs_item_id",
                table: "quest_attempt_reward_logs",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_attempt_reward_logs_quest_attempt_id",
                table: "quest_attempt_reward_logs",
                column: "quest_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_attempts_guild_member_id",
                table: "quest_attempts",
                column: "guild_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_attempts_quest_id",
                table: "quest_attempts",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_maluses_quest_id",
                table: "quest_maluses",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_rewards_item_id",
                table: "quest_rewards",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_quest_rewards_quest_id",
                table: "quest_rewards",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "ix_quests_quest_type_id",
                table: "quests",
                column: "quest_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_recruitment_offers_character_template_id",
                table: "recruitment_offers",
                column: "character_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_recruitment_offers_guild_member_id",
                table: "recruitment_offers",
                column: "guild_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_turns_guild_member_id",
                table: "turns",
                column: "guild_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_token",
                table: "user_sessions",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_equipments");

            migrationBuilder.DropTable(
                name: "guild_inventories");

            migrationBuilder.DropTable(
                name: "quest_attempt_members");

            migrationBuilder.DropTable(
                name: "quest_attempt_reward_logs");

            migrationBuilder.DropTable(
                name: "quest_maluses");

            migrationBuilder.DropTable(
                name: "quest_rewards");

            migrationBuilder.DropTable(
                name: "recruitment_offers");

            migrationBuilder.DropTable(
                name: "turns");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "guild_member_characters");

            migrationBuilder.DropTable(
                name: "quest_attempts");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "character_templates");

            migrationBuilder.DropTable(
                name: "guild_members");

            migrationBuilder.DropTable(
                name: "quests");

            migrationBuilder.DropTable(
                name: "character_classes");

            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.DropTable(
                name: "quest_types");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
