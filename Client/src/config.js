const DEBUG = false;
const CONFIG = Object.freeze(
{
    unknown: "Unknown",
    waitDelayMs: 100,
    
    defaultLabel: 'STARRED',
    // Derived from gmailAPI.getLabels()
    labels: 
    // Note that every thread belongs to exactly 1 category
    // Also, if a thread has been tagged with TRASH it won't appear in any other tags
    [
        "CHAT", "SENT", "INBOX", "IMPORTANT", "TRASH", "DRAFT", "SPAM", "STARRED", "UNREAD", 
        "CATEGORY_FORUMS", "CATEGORY_UPDATES", "CATEGORY_PERSONAL", "CATEGORY_PROMOTIONS", "CATEGORY_SOCIAL", 
    ],
    displayBodiesEvent: "displayBodies",
    reloadInboxEvent: "reloadInbox",
    hideBodiesEvent: "hideBodies",
    rowClassName: "Item",
    threadIdClassName: "threadId",   
    labelSelector: "#labelSelect",
    accountSelector: "#accountSelect",
    loadingWheelSelector: "#bar > img",
    toggleLoadingWheelEvent: "loadingWheelEvent"
})

export { CONFIG, DEBUG };
