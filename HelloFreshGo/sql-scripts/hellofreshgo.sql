-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Apr 19, 2019 at 11:23 AM
-- Server version: 5.7.24
-- PHP Version: 7.2.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hellofreshgo`
--

-- --------------------------------------------------------

--
-- Table structure for table `ratings`
--

DROP TABLE IF EXISTS `Ratings`;
CREATE TABLE IF NOT EXISTS `Ratings` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `RecipeRating` int(11) NOT NULL,
  `RecipeId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `ratings`
--

INSERT INTO `Ratings` (`Id`, `RecipeRating`, `RecipeId`) VALUES
(1, 4, 5),
(2, 5, 3),
(3, 56, 3),
(4, 56, 3);

-- --------------------------------------------------------

--
-- Table structure for table `recipes`
--

DROP TABLE IF EXISTS `Recipes`;
CREATE TABLE IF NOT EXISTS `Recipes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` text,
  `PrepTime` text,
  `Difficulty` int(11) NOT NULL,
  `Vegetarian` smallint(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `recipes`
--

INSERT INTO `Recipes` (`Id`, `Name`, `PrepTime`, `Difficulty`, `Vegetarian`) VALUES
(1, 'Chili Mac', '1 hour', 2, 1),
(3, 'Bacon Cheddar Apple Frittata', '1 hour, 30 minutes', 2, 0),
(4, 'Burrito Pie', '2 hour 15 minutes', 2, 0),
(5, 'Efo Riro', '30 minutes', 1, 1);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
