using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuraLink
{
    static class CommandStrings
    {
        public static readonly string DBCreationString = @"
CREATE TABLE IF NOT EXISTS `NeuralNetwork` (
  `Name` VARCHAR(100) NOT NULL,
  `LearningRate` DOUBLE NOT NULL,
  PRIMARY KEY(`Name`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `Layer` (
  `idLayer` INT NOT NULL AUTO_INCREMENT,
  `ActivationFunction` VARCHAR(50) NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  PRIMARY KEY(`idLayer`),
  INDEX `fk_Layer_NeuralNetwork1_idx` (`Name` ASC) VISIBLE,
  CONSTRAINT `fk_Layer_NeuralNetwork1`
    FOREIGN KEY(`Name`)
    REFERENCES `NeuralNetwork` (`Name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `Neuron` (
  `idNeuron` INT NOT NULL AUTO_INCREMENT,
  `Bias` DOUBLE NOT NULL,
  `idLayer` INT NOT NULL,
  PRIMARY KEY(`idNeuron`),
  INDEX `fk_Neuron_Layer1_idx` (`idLayer` ASC) VISIBLE,
  CONSTRAINT `fk_Neuron_Layer1`
    FOREIGN KEY(`idLayer`)
    REFERENCES `Layer` (`idLayer`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `Dendrite` (
  `idDendrite` INT NOT NULL AUTO_INCREMENT,
  `Weight` VARCHAR(45) NOT NULL,
  `idNeuron` INT NOT NULL,
  PRIMARY KEY(`idDendrite`),
  INDEX `fk_Dendrite_Neuron_idx` (`idNeuron` ASC) VISIBLE,
  CONSTRAINT `fk_Dendrite_Neuron`
    FOREIGN KEY(`idNeuron`)
    REFERENCES `Neuron` (`idNeuron`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;";

        public static readonly string NetworkLoadCmd = @"SELECT idDendrite,Weight,neuron.idNeuron, Bias, layer.idLayer, layer.ActivationFunction , neuralnetwork.LearningRate from dendrite
inner join neuron on dendrite.idNeuron = neuron.idNeuron 
inner join layer on neuron.idLayer = layer.idLayer
inner join neuralnetwork on neuralnetwork.Name = layer.Name";

    }
}
