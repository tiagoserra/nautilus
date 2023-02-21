#!/bin/bash
#----------------------------------------------------------------------------------------------------------------------------------------
#   Script: cg.sh
#   Author: Tiago Serra
#   Date: 30/01/2023 19:00
#   Keywords: Create, file .cs,
#   Comments: how to call script ./cg.sh entityName yes no no
#   Tips: chmod +x cg.sh
#
#   entityName => entity
#   yes => to create unit tests
#   yes => to create dum
#   yes => to create in WebApi
#
#----------------------------------------------------------------------------------------------------------------------------------------

EntityName=$1
WithUnitTest=$2
Dump=$3
WithWebApi=$4

if [[ -z "$EntityName" ]]; then
    echo ERROR: "argument error (EntityName) "
    exit -1
fi

replace() {
    sed -i '' "s/$1/$2/g" $3
}

replaceContentByReplaceIfNotExists() {

    result=$(cat $3 | grep -c "$4")

    if [ $result -eq 0 ]; then
        sed -i '' "s|$1|$2|g" $3
    fi
}

replaceInFile() {

    pathFileDestination=$1
    template=$2

    cp $template $pathFileDestination
    replace '%##%' $EntityName $pathFileDestination
    replace '%#table#%' $EntityName $pathFileDestination
    replace '%#lower#%' $(echo $EntityName | tr '[:upper:]' '[:lower:]') $pathFileDestination
}

domainHandler() {
    echo 'Creating Domain'
    pathDomainDestination='../../Domain'

    replaceInFile $pathDomainDestination'/Entities/'$EntityName'.cs' 'templates/Entity.txt'
    replaceInFile $pathDomainDestination'/Interfaces/Repositories/I'$EntityName'Repository.cs' 'templates/IRepository.txt'
    replaceInFile $pathDomainDestination'/Interfaces/Services/I'$EntityName'Service.cs' 'templates/IService.txt'

    replaceInFile $pathDomainDestination'/Services/'$EntityName'Service.cs' 'templates/Service.txt'
}

infrastructHandler() {
    echo 'Creating Infrastructure'
    pathInfrastructureDestination='../../Infrastructure'

    replaceInFile $pathInfrastructureDestination'/Data/Mappings/'$EntityName'Mapping.cs' 'templates/Mapping.txt'
    replaceInFile $pathInfrastructureDestination'/Data/Repositories/'$EntityName'Repository.cs' 'templates/Repository.txt'

    pathInfrastructureContext=$pathInfrastructureDestination'/Data/Contexts/SqlContext.cs'

    lineTest='public DbSet<'$EntityName'> '$EntityName's { get; set; }'
    replacerNewString=$lineTest'  \n \t//%#DbSet#%'
    replaceContentByReplaceIfNotExists '//%#DbSet#%' "$replacerNewString" $pathInfrastructureContext "$lineTest"

    if [ $Dump = 'yes' ]; then

        echo 'Creating dump entity in Infrastructure Manager'

        pathDestination='../../Infrastructure.Manager/Dumps/'

        replaceInFile $pathDestination'/'$EntityName'Dump.cs' 'Templates/Dump.txt'
    fi
}

unitTestsHandler(){
    echo 'Creating Unit Tests'

    pathUnitTestDestination='../../../tst/UnitTests/Domain/Entities/'
    replaceInFile $pathUnitTestDestination'/'$EntityName'Test.cs' 'templates/EntityUnitTest.txt'

    pathUnitTestDestination='../../../tst/UnitTests/Domain/Services/'
    replaceInFile $pathUnitTestDestination'/'$EntityName'ServiceTest.cs' 'templates/ServiceUnitTest.txt'

    pathUnitTestDestination='../../../tst/UnitTests/Infrastructure/Repositories/'
    replaceInFile $pathUnitTestDestination'/'$EntityName'RepositoryTest.cs' 'templates/RepositoryUniTest.txt'
}

webApiHandler(){

    echo 'Creating WebApi'

    replaceInFile '../../src/WebApi/Controllers/'$EntityName'Controller.cs' 'templates/Controller.txt'

    pathDtos='../../src/WebApi/Dtos/'$EntityName'/'

    if [ ! -f "$pathViews" ]; then
        mkdir -p $pathViews
    fi

    replaceInFile $pathViews'/Get'$EntityName'Dto.cs' 'templates/GetDto.txt'
    replaceInFile $pathViews'/Post'$EntityName'Dto.cs' 'templates/PostDto.txt'
    replaceInFile $pathViews'/Put'$EntityName'Dto.cs' 'templates/PutDto.txt'
}

domainHandler
infrastructHandler

if [ $WithUnitTest = 'yes' ]; then
    unitTestsHandler
fi

if [ $WithWebApi = 'yes' ]; then
    webApiHandler
fi