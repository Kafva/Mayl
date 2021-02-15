const CONFIG = Object.freeze(
{
    unknown: "Unknown",
    
    // Delay between checks when waiting for the current account to be set by AccountSelect.vue
    waitDelayMs: 100,
    
    // Note that every thread belongs to exactly 1 category
    // Also, if a thread has been tagged with TRASH it won't appear in any other tags
    
    defaultLabel: 'INBOX',
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

export {CONFIG};
