import axiosInstance from "./axios-intercepter-instance";

export async function getData(endpoint:string) {
  const response = await axiosInstance.get(`${endpoint}`)
  return response
}

export async function postData(endpoint:string, data: object) {
  const response = await axiosInstance.post(`${endpoint}`, data)
  return response
}

export async function patchData(endpoint:string, data: object) {
  const response = await axiosInstance.patch(`${endpoint}`, data);
  return response
}

export async function deleteData(endpoint: string) {
  const response = await axiosInstance.delete(`${endpoint}`);
  return response;
}