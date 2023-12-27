export const getUsernameAndToken = async () => {
    const response: Response = await fetch("/user/get-username-and-token", { method: 'GET'})
    if (!response.ok) return ['', '']
    const json = await response.json()
    return [json.username, json.apiToken]
}

export const signUp = async (Username: string, Password: string) => {
    const response: Response = await fetch("/user/sign-up", { 
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },      
        body: JSON.stringify({ Username, Password })
    })
    if (!response.ok) {
        const text = await response.text()
        throw new Error(text)
    }
    const json = await response.json()
    return json.apiToken
}

export const login = async (Username: string, Password: string) => {
    const response: Response = await fetch("/user/login", { 
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },      
        body: JSON.stringify({ Username, Password })
    })
    if (!response.ok) {
        const text = await response.text()
        throw new Error(text)
    }
    const json = await response.json()
    return json.apiToken
}

export const logout = async (apiToken: string, logoutAll: boolean = false) => 
    await fetch(`/user/logout?logoutAll=${logoutAll}`, { 
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': apiToken
          }
    })
