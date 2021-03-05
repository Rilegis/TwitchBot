CREATE TABLE `{0}` (
  `id` int(11) NOT NULL COMMENT 'ID',
  `command` varchar(128) NOT NULL COMMENT 'Command name',
  `aliases` varchar(4096) NOT NULL COMMENT 'Commad aliases',
  `type` varchar(128) NOT NULL DEFAULT 'default' COMMENT 'Command type',
  `userLevel` varchar(128) NOT NULL DEFAULT 'any' COMMENT 'What role does the caster needs to have?',
  `cmdArgs` varchar(4096) NOT NULL COMMENT 'Command arguments',
  `cooldown` int(11) NOT NULL DEFAULT '0' COMMENT 'Cooldown until next cast is available',
  `lastUsed` int(11) NOT NULL COMMENT 'Last time the command has been casted',
  `reply` varchar(4096) NOT NULL COMMENT 'Command reply'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `{0}` ADD PRIMARY KEY (`id`);

ALTER TABLE `{0}` MODIFY `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID';