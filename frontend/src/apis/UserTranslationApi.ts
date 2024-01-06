export const getUserTranslation = async (apiToken: string, paragraphId: number, language: string): Promise<string> => {
    const response = await fetch(`/Paragraph/GetUserParagraphTranslation/${paragraphId}/${language}`, { 
        method: 'GET',
        headers: {'Authorization': apiToken }
    })

    if(!response.ok) return ''

    const json = await response.json()
    return (json as any).value
}


export const postUserTranslation = async (apiToken: string, paragraphId: number, Language: string, Value: string): Promise<string> => {
    const response = await fetch(`/Paragraph/SetUserParagraphTranslation/${paragraphId}`, { 
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': apiToken
          },
        body: JSON.stringify({ Language, Value })
    })

    const json = await response.json()
    return (json as any).apiToken
}
