-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 26, 2019 at 06:26 PM
-- Server version: 10.1.38-MariaDB
-- PHP Version: 7.3.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `twitchbot`
--

-- --------------------------------------------------------

--
-- Table structure for table `channels`
--

CREATE TABLE `channels` (
  `id` int(11) NOT NULL COMMENT 'ID',
  `channel` varchar(128) NOT NULL COMMENT 'Channel name',
  `isPG` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'Is the channel on PG mode?',
  `linksAllowed` tinyint(1) NOT NULL DEFAULT '1' COMMENT 'Are links allowed in chat?',
  `isJoined` tinyint(1) NOT NULL COMMENT 'Does the bot needs to join this channel on start?'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `sample_commands`
--

CREATE TABLE `sample_commands` (
  `id` int(11) NOT NULL COMMENT 'ID',
  `command` varchar(128) NOT NULL COMMENT 'Command name',
  `aliases` varchar(4096) NOT NULL COMMENT 'Commad aliases',
  `type` varchar(128) NOT NULL DEFAULT 'default' COMMENT 'Command type',
  `userLevel` varchar(128) NOT NULL DEFAULT 'any' COMMENT 'What role does the caster needs to have?',
  `cmdArgs` varchar(4096) NOT NULL COMMENT 'Command arguments',
  `cooldown` int(11) NOT NULL DEFAULT '0' COMMENT 'Cooldown until next cast is available',
  `lastUsed` varchar(128) NOT NULL COMMENT 'Last time the command has been casted',
  `reply` varchar(4096) NOT NULL COMMENT 'Command reply'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `channels`
--
ALTER TABLE `channels`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `sample_commands`
--
ALTER TABLE `sample_commands`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `channels`
--
ALTER TABLE `channels`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID';

--
-- AUTO_INCREMENT for table `sample_commands`
--
ALTER TABLE `sample_commands`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID';
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
