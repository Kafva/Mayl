import {CONFIG} from './config.js';

const collapseCSS = (minify) =>
{
    return minify ? { display: 'none' } : { display: 'inline-block' }; 
}
const toggleLoadingWheel = (mode) =>
{
  if(mode)
    document.querySelector(CONFIG.loadingWheelSelector).setAttribute("style", "display: inline-block");
  else
    document.querySelector(CONFIG.loadingWheelSelector).setAttribute("style", "display: none");
}

const manageTagOfThread = async (event, endpoint) =>
{
  let trElementIndex= event.path.findIndex( (p) => p.tagName.toUpperCase() == "TR" )
  try
  {
    // Fetch the current account and label
    let userId = getSelected(CONFIG.accountSelector);
    let tag    = getSelected(CONFIG.labelSelector);
    
    let threadId = event.path[trElementIndex].querySelector(`td.${CONFIG.threadIdClassName}`)
      .innerHTML.replace(/[\n\s]*/g,"");
    
    let request = endpoint == "trash" ? `/${userId}/${endpoint}?id=${threadId}` : 
      `/${userId}/${endpoint}?id=${threadId}&tag=${tag}`
    
    let res = await fetch(request, { method: "GET"} );
    
    let resText = await res.text();
    console.log(`${request} : ${resText}`);
    
    // We only remove the element from the UI since refetching the
    // messages for the entire page can take a long time
    if(resText.toUpperCase() == "TRUE") event.path[trElementIndex].remove()
  }
  catch (e) { console.error(e); }
} 

const getDate = (date) => date.replace("T", " ").replace(/\+\d+:\d+/, "");

const getSelected = (selector) =>
{
  let el = document.querySelector(selector);
  let index = el.selectedIndex;
  
  return el.length > index && index >= 0 ? 
    el[index].innerHTML.replace(/[\n\s]*/g,"") : 
    CONFIG.unknown; 
}

export{ collapseCSS, toggleLoadingWheel, manageTagOfThread, getDate, getSelected};
