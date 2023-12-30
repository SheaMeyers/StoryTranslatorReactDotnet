export const getBooks = async (): Promise<string[]> => {
    const response = await fetch("/books", { method: 'GET'})
    const responseJson = await response.json();
    return responseJson.titles
}
