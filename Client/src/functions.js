const collapseCSS = (minify) =>
{
    return minify ? { display: 'none' } : { display: 'inline-block' }; 
}

export{ collapseCSS };
