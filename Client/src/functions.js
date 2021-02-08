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

const getDate = (date) => date.replace("T", " ").replace(/\+\d+:\d+/, "");

export{ collapseCSS, toggleLoadingWheel, getDate};
