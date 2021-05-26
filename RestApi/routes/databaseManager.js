'use strict';

const AWS = require('aws-sdk');
let dynamo = new AWS.DynamoDB.DocumentClient();

const TABLE_NAME = process.env.TABLE_NAME;

exports.getUserItems = userId => {
    const params = {
        TableName: TABLE_NAME,
        KeyConditionExpression: '#user_id = :user_id_val',
        ExpressionAttributeNames: {
            '#user_id': 'userId'
        },
        ExpressionAttributeValues: {
            ':user_id_val': userId
        },
        ProjectionExpression: 'started,finished,startLocation,finishLocation,created',
        Select: 'SPECIFIC_ATTRIBUTES',
        ScanIndexForward: false
    };

    return dynamo
        .query(params)
        .promise()
        .then(result => result.Items);
};

exports.saveItem = item => {
    const params = {
        TableName: TABLE_NAME,
        Item: item
    };

    return dynamo
        .put(params)
        .promise()
        .then(() => item.started);
};

exports.getItem = (userId, started) => {
    const params = {
        Key: { userId, started },
        TableName: TABLE_NAME,
        ProjectionExpression: 'started,finished,startLocation,finishLocation,created,points',
        Select: 'SPECIFIC_ATTRIBUTES'
    };

    return dynamo
        .get(params)
        .promise()
        .then(result => result.Item);
};

exports.deleteItem = (userId, started) => {
    const params = {
        Key: { userId, started },
        TableName: TABLE_NAME
    };

    return dynamo.delete(params).promise();
};

exports.updateItem = (userId, started, paramsName, paramsValue) => {
    const params = {
        TableName: TABLE_NAME,
        Key: { userId, started },
        ConditionExpression: 'attribute_exists(started)',
        UpdateExpression: 'set ' + paramsName + ' = :v',
        ExpressionAttributeValues: {
            ':v': paramsValue
        },
        ReturnValues: 'ALL_NEW'
    };

    return dynamo
        .update(params)
        .promise()
        .then(response => response.Attributes);
};