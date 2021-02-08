const collapseCSS = (minify) =>
{
    return minify ? { display: 'none' } : { display: 'auto' }; 
}

export{ collapseCSS };
