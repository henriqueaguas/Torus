# GlobalManager DDOL

playerAbilities: Set<Ability.AbilityType>

house1ability_tag: String // position of "Boot" in village
house1abs: AbilityType // boot

house2ability_tag: String // position of "Grab" in village
house2abs: AbilityType // grab

lastCompletedHouse: int

tryEnterHouse(houseID)
    check if has required abilities
    PathPointer.

completeHouse(houseID)
    require houseID >= lastCompletedHouse
    lastCompletedHouse = houseID
    nexthouse = houseID + 1
    Find(house2ability_tag).enable()
    PathPointer.setTarget(house2ability_tag)

captureAbility(abilityGo)
    playerAbilities.Add(abilityGo.ability)
    Destroy(abilityGo)
    PathPointer.setTarget(house_2_tag)


--- Portal ---

onVillagePortalEnter
    require GM.tryEnterHouse(1)
    change scene to house

onPKLevelFinalPortalEnter   
    completeHouse(1)


--- Ability ---

onCaptureAbility
    GM.captureAbility(this)



# Abilities

-Iterar pelas abilities e destruir as que nao sao da next house
-ser o código da Firefly a procurar o próximo target (apenas se estiver na village). Ou seja, no Update() do firefly se estiveres na village e sem target vais ver se o GlobalManager tem um target para ti. 
    