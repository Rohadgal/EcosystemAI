# Ecosystem Simulator: Survival of the Fittest

Welcome to "Ecosystem Simulator: Survival of the Fittest"! This project is a 3D ecosystem simulation game developed in Unity, where players have the opportunity to explore the intricacies of an ever-changing environment.

## Description
In "Ecosystem Simulator: Survival of the Fittest," players are tasked with overseeing a procedurally generated ecosystem. With the power to control population sizes and behaviors of various animal species, players must navigate the delicate balance between life and death, evolution and extinction.

## Features
- **Dynamic Population Control:** Players can adjust the number of animals in the ecosystem, ranging from 2 to 12 for each species: lions, dogs, cats, and chickens.
- **Procedurally Generated Environments:** Each level offers a unique environment, generated procedurally to provide endless replayability and challenges.

       for (int i = 0; i < gridWidth; i++) {
            for (int j = 0; j < gridDepth; j++) {
                if (canGenerateCell) {
                    setRandomCubes(i, j);
                }
            }
        }
        // Copy the array values of the random initial cubes to a bool array to know if they are alive or dead
        copyArray();

        for(int it=0; it<iterations; it++) {
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridDepth; j++) {
                    // Check if the cell matrix is empty before creating a new one
                    if (canGenerateCell) {
                        //check neighbors
                        int numOfNeighbors = checkNeighbors(i, j);
                        switch (cellsArrayMap[i, j]) {
                            case CellType.Grass:
                                cellsArray[i, j].GetComponent<CubeCell>().setCube((numOfNeighbors >= numGrass) ? CellType.Grass : CellType.Water);
                                break;
                            case CellType.Water:
                                cellsArray[i, j].GetComponent<CubeCell>().setCube((numOfNeighbors >= numWater) ? CellType.Water : CellType.Grass); 
                                break;
                            default: Debug.Log("Error with cell type"); break;
                        }
                    }

                }
            }   
        }
  
- **Realistic Animal Behaviors:** Animals exhibit lifelike behaviors such as thirst, hunger, and the urge to reproduce, requiring players to monitor and manage their needs.
  
       void survivalSystem() {
        _animal._gene.hungerSystem(_animal, hungerIncrement);
        _animal._gene.thirstSystem(_animal, thirstIncrement);
        _animal._gene.urgeSystem(_animal, urgeIncrement);

        if (_animal.getHunger() > _animal._gene.feelHungry) {
            isHungry = true;
            isSatisfied = false;
        }
        if (_animal.getThirst() > _animal._gene.feelThirst) {
            isThirsty = true;
            isSatisfied = false;
        }
        if (_animal.getUrge() > _animal._gene.feelUrge) {
            hasUrge = true;
        }
      }
  
- **Genetic Inheritance and Mutation:** Witness the genetic inheritance and occasional mutation as animals reproduce, introducing variability and potential advantages.

      Gene GenerateOffspringGene(Gene partnerGene) {
        Gene offspringGene = new Gene();

        float randomValueHunger = 0.5f;
        float randomValueThirst = 0.5f;
        float randomValueUrge = 0.5f;

        if(Random.Range(0,100) < 25) {
            randomValueHunger = Random.Range(0.2f, 1.5f);
            randomValueThirst = Random.Range(0.2f, 1.5f);
            randomValueUrge = Random.Range(0.2f, 1.5f);
        }

        offspringGene.feelHungry = (_gene.feelHungry + partnerGene.feelHungry) * randomValueHunger;
        offspringGene.feelThirst = (_gene.feelThirst + partnerGene.feelThirst) * randomValueThirst;
        offspringGene.feelUrge = (_gene.feelUrge + partnerGene.feelUrge) * randomValueUrge;

        return offspringGene;
      }
  
- **Inter-Species Interactions:** Experience the intricate predator-prey relationships between animals, influencing the ecosystem's balance.
- **Environmental Factors:** Adapt to environmental changes like climate variations, resource availability, and natural disasters.
- **Educational Insights:** Gain valuable insights into ecology and evolution while enjoying an engaging and interactive gameplay experience.

## Installation
1. Clone the repository to your local machine.
2. Open the project in Unity.
3. Explore and enjoy the ecosystem simulation!

## Contributing
Contributions are welcome! Feel free to submit bug reports, feature requests, or pull requests.

## License
This project is licensed under the [Creative Commons License](LICENSE).

## Credits
- Developed by Rodrigo Adrian Galindo Frias (https://github.com/Rohadgal)
- Animal models and textures sourced from [Asset Store](https://assetstore.unity.com/) 
- Additional resources and references used for development [itch.io](https://itch.io/) and [freesound](https://freesound.org/)

## Contact
For questions or inquiries, please contact [gal.frias@gmail.com].

Enjoy playing "Ecosystem Simulator: Survival of the Fittest"!
