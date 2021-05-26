'use strict';

const databaseManager = require('./databaseManager');

exports.handler = async (event) => {

    const httpMethod = event.requestContext.http.method;
    const userId = getUserId(event);
    const itemId = getItemId(event);
    const data = getData(event);

    if (itemId) {
        switch (httpMethod) {
            case 'GET':
                return getItem(userId, itemId);
            case 'PUT':
                return updateItem(userId, itemId, data);
            case 'DELETE':
                return deleteItem(userId, itemId);
        }
    } else {
        switch (httpMethod) {
            case 'GET':
                return getUserItems(userId);
            case 'POST':
                return saveItem(userId, data);
        }
    }

    return createResponse(404);
};

function getUserItems(userId) {
    return databaseManager.getUserItems(userId).then(response => {
        return createResponse(200, response);
    });
}

function saveItem(userId, data) {
    const item = { ...data };
    item.userId = userId;
    item.created = Date.now() / 1000 | 0;

    data.points.sort((a, b) => a.timestamp - b.timestamp);
    item.started = data.points[0].timestamp;
    item.finished = data.points[data.points.length - 1].timestamp;
    item.startLocation = { latitude: data.points[0].latitude, longitude: data.points[0].longitude };
    item.finishLocation = { latitude: data.points[data.points.length - 1].latitude, longitude: data.points[data.points.length - 1].longitude };

    return databaseManager.saveItem(item).then(response => {
        return createResponse(200, response);
    });
}

function getItem(userId, itemId) {
    return databaseManager.getItem(userId, itemId).then(response => {
        if (!response) {
            return createResponse(404);
        }

        return createResponse(200, response);
    });
}

function updateItem(userId, itemId, data) {
    const paramName = data.paramName;
    const paramValue = data.paramValue;

    if (paramName === "created") {
        return createResponse(400);
    }

    return databaseManager.updateItem(userId, itemId, paramName, paramValue).then(response => {
        return createResponse(200, response);
    }, () => {
        return createResponse(400);
    });
}

function deleteItem(userId, itemId) {
    return databaseManager.deleteItem(userId, itemId).then(response => {
        return createResponse(200);
    });
}

function getUserId(event) {
    return event.requestContext.authorizer.jwt.claims.sub;
}

function getItemId(event) {
    return event.pathParameters ? Number(event.pathParameters.id) : null;
}

function getData(event) {
    return event.body ? JSON.parse(event.body) : null;
}

function createResponse(statusCode, message) {
    return {
        statusCode: statusCode,
        body: JSON.stringify(message)
    };
}