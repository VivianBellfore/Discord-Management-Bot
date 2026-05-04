-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.11.13-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.5.0.6677
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for discord_management_db
CREATE DATABASE IF NOT EXISTS `discord_management_db` /*!40100 DEFAULT CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci */;
USE `discord_management_db`;

-- Dumping structure for table discord_management_db.blocked_text
CREATE TABLE IF NOT EXISTS `blocked_text` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `text` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  KEY `fk_blocked_text_guild_id` (`guild_id`),
  CONSTRAINT `fk_blocked_text_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.bot_user_bans
CREATE TABLE IF NOT EXISTS `bot_user_bans` (
  `user_id` bigint(24) unsigned NOT NULL,
  `reason` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci COMMENT='Users that are not allowed to use the bot.';

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.discord_items
CREATE TABLE IF NOT EXISTS `discord_items` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `card_url` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `item_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.factions
CREATE TABLE IF NOT EXISTS `factions` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT 'Faction',
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT 'A new faction.',
  `owner_id` bigint(24) unsigned NOT NULL,
  `admin_id` bigint(24) unsigned NOT NULL,
  `guild_id` bigint(24) unsigned NOT NULL,
  `max_member` int(11) NOT NULL DEFAULT 3,
  `max_channel` int(11) NOT NULL DEFAULT 1,
  `max_ranks` int(11) NOT NULL DEFAULT 0,
  `points` bigint(20) NOT NULL DEFAULT 0,
  `category_id` bigint(24) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_factions_guild_id` (`guild_id`),
  CONSTRAINT `fk_factions_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.factions_channel
CREATE TABLE IF NOT EXISTS `factions_channel` (
  `faction_id` int(11) NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  `channel_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'text,  voice',
  `is_public` int(11) NOT NULL,
  KEY `fk_faction_channel_faction_id` (`faction_id`),
  CONSTRAINT `fk_faction_channel_faction_id` FOREIGN KEY (`faction_id`) REFERENCES `factions` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.factions_user
CREATE TABLE IF NOT EXISTS `factions_user` (
  `faction_id` int(11) NOT NULL,
  `user_id` bigint(24) unsigned NOT NULL,
  `rank_id` int(11) NOT NULL,
  KEY `fk_faction_user_faction_id` (`faction_id`),
  KEY `fk_faction_user_user_id` (`user_id`),
  CONSTRAINT `fk_faction_user_faction_id` FOREIGN KEY (`faction_id`) REFERENCES `factions` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_faction_user_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_profile` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_channel
CREATE TABLE IF NOT EXISTS `guild_channel` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `system` bigint(24) unsigned NOT NULL DEFAULT 0,
  `news` bigint(24) unsigned NOT NULL DEFAULT 0,
  `events` bigint(24) unsigned NOT NULL DEFAULT 0,
  `wowchar` bigint(24) unsigned NOT NULL DEFAULT 0,
  `ticket` bigint(24) unsigned NOT NULL DEFAULT 0,
  `tempvoice` bigint(24) unsigned NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_channel_rules
CREATE TABLE IF NOT EXISTS `guild_channel_rules` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  `title` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `text` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `field_1` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `field_2` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `field_3` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `url_string` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `color` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_data
CREATE TABLE IF NOT EXISTS `guild_data` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `register_date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'DateTime.Now.ToShortDateString()',
  `language` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `role_member` bigint(24) unsigned NOT NULL DEFAULT 0,
  `role_mod` bigint(24) unsigned NOT NULL DEFAULT 0,
  `role_admin` bigint(24) unsigned NOT NULL DEFAULT 0,
  `points_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT 'Punkte',
  `channel_logs` bigint(24) unsigned NOT NULL DEFAULT 0,
  `channel_news` bigint(24) unsigned NOT NULL DEFAULT 0,
  `channel_events` bigint(24) unsigned NOT NULL DEFAULT 0,
  `category_ticket` bigint(24) unsigned NOT NULL DEFAULT 0,
  `category_voice` bigint(24) unsigned NOT NULL DEFAULT 0,
  `invite_link` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '',
  `wordfilter` tinyint(4) NOT NULL DEFAULT 0,
  `deletemessage` tinyint(4) NOT NULL DEFAULT 0,
  `gatedcommunity` tinyint(4) NOT NULL DEFAULT 0,
  `ticketsactive` tinyint(4) NOT NULL DEFAULT 0,
  `econemy` tinyint(4) NOT NULL DEFAULT 0,
  `tempvoice` tinyint(4) NOT NULL DEFAULT 0,
  `halloween` tinyint(4) NOT NULL DEFAULT 0,
  `wowactive` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`guild_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_pending_actions
CREATE TABLE IF NOT EXISTS `guild_pending_actions` (
  `type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `guild_id` bigint(24) unsigned NOT NULL,
  `user_id` bigint(24) unsigned NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  KEY `fk_pending_actions_user_id` (`user_id`),
  KEY `fk_pending_actions_guild_id` (`guild_id`),
  CONSTRAINT `fk_pending_actions_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_pending_actions_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_profile` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_reaction_messages
CREATE TABLE IF NOT EXISTS `guild_reaction_messages` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  `message_id` bigint(24) unsigned NOT NULL,
  `event_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  KEY `fk_guild_reaction_message_guild_id` (`guild_id`),
  CONSTRAINT `fk_guild_reaction_message_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_special_roles
CREATE TABLE IF NOT EXISTS `guild_special_roles` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `role_id` bigint(24) unsigned NOT NULL,
  `role_type` varchar(50) NOT NULL,
  KEY `fk_guild_special_roles_guild_id` (`guild_id`),
  CONSTRAINT `fk_guild_special_roles_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_temp_voice
CREATE TABLE IF NOT EXISTS `guild_temp_voice` (
  `user_id` bigint(24) unsigned NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  `guild_id` bigint(24) unsigned NOT NULL,
  `time` bigint(20) NOT NULL DEFAULT 0 COMMENT 'DateTime.Now.ToBinary() / DateTime.FromBinary(time)',
  KEY `fk_tempvoice_userid` (`user_id`),
  KEY `fk_tempvoice_guildid` (`guild_id`),
  CONSTRAINT `fk_tempvoice_guildid` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tempvoice_userid` FOREIGN KEY (`user_id`) REFERENCES `user_profile` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.guild_user_roles
CREATE TABLE IF NOT EXISTS `guild_user_roles` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `role_id` bigint(24) unsigned NOT NULL,
  KEY `fk_user_roles_guild_id` (`guild_id`),
  CONSTRAINT `fk_user_roles_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.reminder_public
CREATE TABLE IF NOT EXISTS `reminder_public` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `guild_id` bigint(24) unsigned NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  `user_id` bigint(24) unsigned NOT NULL,
  `time` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `title` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `picture` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `color` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT 'grey',
  `date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '',
  `role_ids` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `weekday` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '',
  `daily` int(11) NOT NULL DEFAULT 0,
  `duration` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.reports
CREATE TABLE IF NOT EXISTS `reports` (
  `user_id` bigint(24) unsigned NOT NULL,
  `reason` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `comment` text CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `reporter_id` bigint(24) unsigned NOT NULL,
  `guild_id` bigint(24) unsigned NOT NULL,
  KEY `fk_reports_guild_id` (`guild_id`),
  CONSTRAINT `fk_reports_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.tickets
CREATE TABLE IF NOT EXISTS `tickets` (
  `guild_id` bigint(24) unsigned NOT NULL,
  `channel_id` bigint(24) unsigned NOT NULL,
  `user_id` bigint(24) unsigned NOT NULL,
  KEY `fk_tickets_user_id` (`user_id`),
  KEY `fk_tickets_guild_id` (`guild_id`),
  CONSTRAINT `fk_tickets_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tickets_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_profile` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.user_dc_inventory
CREATE TABLE IF NOT EXISTS `user_dc_inventory` (
  `user_id` bigint(24) unsigned NOT NULL,
  `item_id` int(11) NOT NULL,
  `amount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.user_event_date
CREATE TABLE IF NOT EXISTS `user_event_date` (
  `user_id` bigint(24) unsigned NOT NULL,
  `event_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'DateTime.Now.ToString("o")',
  KEY `fk_event_date_user_id` (`user_id`),
  CONSTRAINT `fk_event_date_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_profile` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.user_guild_points
CREATE TABLE IF NOT EXISTS `user_guild_points` (
  `user_id` bigint(24) unsigned NOT NULL,
  `guild_id` bigint(24) unsigned NOT NULL,
  `points` bigint(24) unsigned NOT NULL,
  KEY `fk_guild_points_user_id` (`user_id`),
  KEY `fk_guild_points_guild_id` (`guild_id`),
  CONSTRAINT `fk_guild_points_guild_id` FOREIGN KEY (`guild_id`) REFERENCES `guild_data` (`guild_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_guild_points_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_profile` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.user_halloween_candy
CREATE TABLE IF NOT EXISTS `user_halloween_candy` (
  `user_id` bigint(24) unsigned NOT NULL,
  `candy_id` int(11) NOT NULL,
  `amount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table discord_management_db.user_profile
CREATE TABLE IF NOT EXISTS `user_profile` (
  `user_id` bigint(24) unsigned NOT NULL,
  `language` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `points` int(11) NOT NULL DEFAULT 0,
  `winter_points` int(11) NOT NULL DEFAULT 0,
  `winter_tickets` int(11) NOT NULL DEFAULT 0,
  `block_bot_dm` tinyint(4) NOT NULL DEFAULT 0,
  `password` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL,
  `halloween_date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '2025-09-25T14:15:00' COMMENT 'DateTime.Now.ToString("o")',
  `halloween_action` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '2025-09-25T14:15:00' COMMENT 'DateTime.Now.ToString("o")',
  `halloween_protection` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '2025-09-25T14:15:00' COMMENT 'DateTime.Now.ToString("o")',
  PRIMARY KEY (`user_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- Data exporting was unselected.



/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
