"use client";

import { postData } from "@/utils/api";
import { Anchor, Button, Container, Group, Paper, PasswordInput, Text, TextInput } from "@mantine/core";
import { hasLength, isEmail, useForm } from "@mantine/form";
import { notifications } from "@mantine/notifications";
import { useRouter } from "next/navigation";


export default function Register() {
    const router = useRouter();
    
    const form = useForm({
        mode: "controlled",
        initialValues: { fullName: "", email: "", password: ""},
        validate: {
            fullName: hasLength({ min: 6 }, "En az 6 karakter olmalı"),
            email: isEmail("Geçerli bir e-posta adresi girin"),
            password: hasLength({ min: 8 }, "En az 8 karakter olmalı"),
        },
    });


    const handleSubmit = async (values: typeof form.values) => {
        try{
            const res = await postData("api/Auth/Register", {email: values.email, password: values.password, fullName: values.fullName});
            if(res.data.status === "Success") {
                notifications.show({
                    title: "Kayıt Başarılı",
                    message: "Hesabınız başarıyla oluşturuldu.",
                    color: "green",
                });                
                router.push("/login");
            }
            else {
                notifications.show({
                    title: "Kayıt Başarısız",
                    message: res.data.message,
                    color: "red",
                });
            }
        }
        catch (error) {
            console.error("Login failed:", error);
        }
    };

 
    return (
        <div>
            <form onSubmit={form.onSubmit(handleSubmit)}>
                <Container size={420} my={40}>
                    <Text className="text-center" size="32px" fw={700} mb={15}>
                        Bir hesap oluştur
                    </Text>

                    <Text className="text-center" size="sm" c={"dimmed"}>
                        Zaten bir hesabınız var mı? <Anchor href="/login"> Giriş yapın </Anchor>
                    </Text>

                    <Paper withBorder shadow="sm" p={22} mt={30} radius="md">
                        <TextInput
                            {...form.getInputProps("fullName")}
                            label="Kullanıcı Adı"
                            placeholder="Kullanıcı Adınız"
                            />
                        <TextInput
                            {...form.getInputProps("email")}
                            label="Email"
                            placeholder="Email"
                            mt="md"
                            />
                        <PasswordInput 
                            {...form.getInputProps("password")}
                            label="Şifre" 
                            placeholder="Şifreniz" 
                            mt="md" 
                            />
                                                    
                        <Button fullWidth mt="xl" radius="md" type="submit" >
                            Kayıt Ol
                        </Button>
                    </Paper>
                </Container>
            </form>
        </div>
    );
}
