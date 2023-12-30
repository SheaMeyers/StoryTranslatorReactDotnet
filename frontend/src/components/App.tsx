import { useEffect, useState } from 'react'
import { getUsernameAndToken } from '../UserApi'
import Header from './Header'
import Book from './Book'


const App = () => {
  const [username, setUsername] = useState<string>('')
  const [apiToken, setApiToken] = useState<string>('')

  const updateUsernameAndApiToken = (username: string, apiToken: string) => {
    setUsername(username)
    setApiToken(apiToken)
  }

  useEffect(() => {
    const fetchData = async () => {
      const [retrievedUsername, retrievedApiToken] = await getUsernameAndToken()
      if (retrievedUsername && retrievedApiToken) {
        setUsername(retrievedUsername)
        setApiToken(retrievedApiToken)
      }
    }
    fetchData()  
  }, [])

  return (
    <>
     <Header apiToken={apiToken} username={username} updateUsernameAndApiToken={updateUsernameAndApiToken} /> 
     <Book />
    </>
  )
}

export default App
