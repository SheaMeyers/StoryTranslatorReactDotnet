export const getUsernameAndToken = async () => {
    const response: Response = await fetch("/user/get-username-and-token", { method: 'GET'})
    if (!response.ok) return ['', '']
    const json = await response.json()
    return [json.username, json.apiToken]
}
