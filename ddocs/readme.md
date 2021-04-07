# Setup

Using either [setup.ps1](setup.ps1) or [setup.sh](setup.sh) will use [curl](https://curl.se/) to try and create the [ddoc.json](ddoc.json) on the provided url.

This means the contents of this [ddoc.json](ddoc.json) will be added to the design documents of the database.

The database ddoc will have it's name choosen from the `_id` property inside the [ddoc.json](ddoc.json).

## Important

The url should be fully qualified with password and user. 

eg: User = `admin`, Password = `password`, Host = `localhost:5984`, Database = `my_database`

the complete url then is `https://admin:password@localhost:5984/my_database`

# Views Overview

For each method there is an `if (doc.split_discriminator == 'type')` which if true emit will be executed, type can be `identity.user` or `identity.role` the if is ommited for brevity.

To call a view you first use the `doc name` then the `view id` below are only the view ids you call and their results.

In code there are some `Internal` namespaces that containt a static `Views` class and a `View` class that is used to call views, you can replace them or use them in case you need functionality that is not provided but do so with care.

## user
Returns the count of all user documents.  
When `reduce` is `false` it returns the document id.

    map: emit(doc._id, doc._rev);
    reduce: _count

## user.normalized_username
Returns the `normalized_username` of a user as key and `rev` as value.

    map: emit(doc.normalized_username, doc._rev);

## user.normalized_email
Returns the `normalized_email` of a user as key and `rev` as value.

    map: emit(doc.email.normalized, doc._rev);

## user.roles.normalized_name
Returns every role `normalized_name` as key for each user and `rev` as value.

    map: doc.roles.forEach(role => emit(role.normalized_name, doc._rev));

## user.claims
Returns every claim as an array key `[type, value]` for each user and `rev` as value.

    map: doc.claims.forEach(claim => emit([claim.type, claim.value], doc._rev);

## user.logins
Returns every login as an array key `[provider, key]` for each user and `rev` as value.

    map: doc.logins.forEach(login => emit([login.provider, login.key],doc._rev);

## role
Returns the count of all role documents.  
When `reduce` is `false` it returns the document id.

    map: emit(doc._id, doc._rev);
    reduce: _count,

## role.normalized_name
Returns the `normalized_name` of a role as key and `rev` as value.

    map: emit(doc.normalized_name, doc._rev);
