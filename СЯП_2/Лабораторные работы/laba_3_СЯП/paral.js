async function limitRequests(urls, limit) {
    const results = [];
    

    for (let i = 0; i < urls.length; i += limit) {
        const chunk = urls.slice(i, i + limit);
        
     
        const chunkResults = await Promise.all(
            chunk.map(url => 
                fetch(url).then(res => res.json())
            )
        );
        
        results.push(...chunkResults);
        console.log(`Загружена пачка с ${i} по ${i + chunk.length}`);
    }
    
    return results;
}
