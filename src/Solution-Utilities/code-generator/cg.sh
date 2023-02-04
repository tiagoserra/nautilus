#!/bin/bash
#----------------------------------------------------------------------------------------------------------------------------------------
#   Script: cg.sh
#   Author: Tiago Serra
#   Date: 30/01/2023 19:00
#   Keywords: Create, file .cs,
#   Comments: how to call script ./cg.sh entityName
#   Tips: chmod +x cg.sh
#
#   entityName => entity
#
#----------------------------------------------------------------------------------------------------------------------------------------

EntityName=$1

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
}

domainHandler() {
    echo 'Creating Domain'
    pathDomainDestination='../../Domain'

    replaceInFile $pathDomainDestination'/Entities/'$EntityName'.cs' 'templates/Entity.txt'
    replaceInFile $pathDomainDestination'/Interfaces/Repositories/I'$EntityName'Repository.cs' 'templates/IRepository.txt'
    replaceInFile $pathDomainDestination'/Interfaces/Services/I'$EntityName'Service.cs' 'templates/IService.txt'

    replaceInFile $pathDomainDestination'/Services/'$EntityName'Service.cs' 'templates/Service.txt'

    pathUnitTestDestination='../../../tst/UnitTests/Domain/Entities/'

    replaceInFile $pathUnitTestDestination'/'$EntityName'UnitTest.cs' 'templates/EntityUnitTest.txt'
}

infrastructHandler() 

    echo 'Creating Infrastructure'
    pathInfrastructureDestination='../../Infrastructure'

    replaceInFile $pathInfrastructureDestination'/Data/Mappings/'$EntityName'Mapping.cs' 'templates/Mapping.txt'
    replaceInFile $pathInfrastructureDestination'/Data/Repositories/'$EntityName'Repository.cs' 'templates/Repository.txt'

    pathInfrastructureContext=$pathInfrastructureDestination'/Data/Contexts/SqlServerContext.cs'

    lineTest='public DbSet<'$EntityName'> '$EntityName's { get; set; }'
    replacerNewString=$lineTest'  \n \t//%#DbSet#%'
    replaceContentByReplaceIfNotExists '//%#DbSet#%' "$replacerNewString" $pathInfrastructureContext "$lineTest"
}

domainHandler
infrastructHandler
