import { useEffect, useState } from 'react'
import { getUsernameAndToken } from '../api'
import Header from './Header'


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
    </>
  )
}

export default App
