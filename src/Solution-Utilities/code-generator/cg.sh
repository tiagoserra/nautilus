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
    echo ERROR: "argument error (2 - EntityName) "
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
}

domainHandler() {
    echo 'Creating module in Domain'
    pathDomainDestination='../Domain'

    replaceInFile $pathDomainDestination'/Entities/'$EntityName'.cs' 'templates/Entity.txt'
    replaceInFile $pathDomainDestination'/Interfaces/Repositories/I'$EntityName'Repository.cs' 'templates/IRepository.txt'
    replaceInFile $pathDomainDestination'/Interfaces/Services/I'$EntityName'Service.cs' 'templates/IService.txt'

    replaceInFile $pathDomainDestination'/Services/'$EntityName'Service.cs' 'templates/Service.txt'
}

domainHandler
